<template>
  <!-- 开票表单：选择来源（订单/交货）、编辑发票头与行、提交创建发票 -->
  <div class="page invoice-form">
    <a-page-header title="新建发票" @back="goBack" />

    <a-card title="来源选择" :bordered="false" style="margin-bottom: 12px">
      <InvoiceSourceSelector @resolved="onSourceResolved" />
    </a-card>

    <a-card title="发票信息" :bordered="false" style="margin-bottom: 12px">
      <a-form :model="form" :label-col="{ span: 5 }" :wrapper-col="{ span: 19 }">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="客户" required>
              <AccountSelect v-model="(form as any).customerId" placeholder="选择来源后自动填充，可手工修改" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="客户名称">
              <a-input v-model:value="form.customerName" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="默认税率">
              <a-input-number v-model:value="form.defaultTaxRate" :step="0.01" :min="0" :max="1" :precision="4" style="width: 100%" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="是否电子发票">
              <a-switch v-model:checked="form.isElectronic" />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-card>

    <a-card title="发票明细" :bordered="false">
      <InvoiceLinesEditor v-model="lines" />
    </a-card>

    <div class="actions">
      <a-space>
        <a-button @click="goBack">取消</a-button>
        <a-button type="primary" :loading="submitting" @click="onSubmit">提交</a-button>
      </a-space>
    </div>
  </div>
</template>

<script setup lang="ts">
// 开票表单：从订单/交货拉取明细并可编辑，提交到 /api/Invoices
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { message } from 'ant-design-vue';
import { http } from '@/utils/request';
import InvoiceLinesEditor, { type InvoiceItemEdit } from '@/components/invoices/InvoiceLinesEditor.vue';
import InvoiceSourceSelector, { type InvoiceItemDraft } from '@/components/invoices/InvoiceSourceSelector.vue';
import AccountSelect from '@/components/select/AccountSelect.vue'

const router = useRouter();

type SrcType = 'order' | 'delivery';

const form = ref({
  customerId: '',
  customerName: '',
  defaultTaxRate: 0.13,
  isElectronic: false,
});

const sourceType = ref<SrcType>('order');
const sourceId = ref<string | undefined>(undefined);
const sourceNumber = ref<string | undefined>(undefined);

const lines = ref<InvoiceItemEdit[]>([]);

function goBack() {
  router.back();
}

function onSourceResolved(payload: { sourceType: SrcType; sourceId: string; sourceNumber?: string; items: InvoiceItemDraft[]; customerId?: string; customerName?: string }) {
  sourceType.value = payload.sourceType;
  sourceId.value = payload.sourceId;
  sourceNumber.value = payload.sourceNumber;
  if (payload.customerId) form.value.customerId = payload.customerId;
  if (payload.customerName) form.value.customerName = payload.customerName;
  // 映射行
  lines.value = payload.items.map((x, idx) => ({
    _rowKey: Math.random().toString(36).slice(2),
    productCode: x.productCode,
    productName: x.productName,
    specification: x.specification,
    unit: x.unit,
    quantity: x.quantity,
    unitPrice: x.unitPrice,
    taxRate: x.taxRate,
    sortOrder: x.sortOrder ?? idx + 1,
    remark: x.remark,
  }));
}

const submitting = ref(false);

async function onSubmit() {
  if (!form.value.customerId) { message.warning('请选择客户'); return; }
  if (!lines.value.length) { message.warning('请至少添加一行明细'); return; }

  // 组装 CreateInvoiceRequest（注意：大小写按后端 DTO）
  const payload: any = {
    customerId: form.value.customerId,
    customerName: form.value.customerName,
    sourceType: sourceType.value === 'order' ? 0 : 1, // InvoiceSourceType 枚举：order=0, delivery=1（与后端保持一致）
    sourceId: sourceId.value,
    sourceNumber: sourceNumber.value,
    defaultTaxRate: form.value.defaultTaxRate,
    isElectronic: form.value.isElectronic,
    items: lines.value.map((r, idx) => ({
      productCode: r.productCode,
      productName: r.productName,
      specification: r.specification,
      unit: r.unit,
      quantity: Number(r.quantity || 0),
      unitPrice: Number(r.unitPrice || 0),
      taxRate: Number(r.taxRate ?? form.value.defaultTaxRate ?? 0),
      sortOrder: r.sortOrder ?? idx + 1,
      remark: r.remark,
    })),
  };

  submitting.value = true;
  try {
    // 直接调用，不经过 invoices.ts，避免本批超过 5 个文件
    const resp = await http.post<any>('/api/Invoices', payload);
    // 兼容统一响应二次包裹
    const data = resp && resp.data ? resp.data : resp;
    const id = data?.id || data?.data?.id;
    message.success('开票成功');
    if (id) router.push(`/invoices/${id}`);
  } catch (err: any) {
    message.error(err?.message || '开票失败');
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped>
.actions { margin-top: 12px; display: flex; justify-content: flex-end; }
</style>
