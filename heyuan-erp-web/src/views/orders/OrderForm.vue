<template>
  <!-- 订单表单页：新建/编辑，支持行增删改与确认/反审操作 -->
  <div class="page order-form">
    <a-card :title="pageTitle" :loading="loading" :bordered="false">
      <a-form :model="form" layout="vertical" @submit.prevent>
        <a-row :gutter="16">
          <a-col :span="8">
            <a-form-item label="客户" name="accountId" required>
              <AccountSelect v-model="(form as any).accountId" placeholder="按编码/名称选择客户" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="下单日期" name="orderDate" required>
              <a-date-picker v-model:value="form.orderDate" value-format="YYYY-MM-DD" style="width: 100%" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="币种" name="currency">
              <a-input v-model:value="form.currency" placeholder="例如 CNY" />
            </a-form-item>
          </a-col>
        </a-row>

        <a-form-item label="备注" name="remark">
          <a-textarea v-model:value="form.remark" :rows="2" />
        </a-form-item>

        <a-divider>订单明细</a-divider>
        <a-table :data-source="form.lines" :pagination="false" row-key="id">
          <a-table-column title="#" :customRender="({index}) => index! + 1" width="60" />
          <a-table-column title="产品" width="280">
            <template #default="{ record }">
              <ProductSelect v-model="record.productId" placeholder="按编码/名称选择产品" />
            </template>
          </a-table-column>
          <a-table-column title="数量" width="140">
            <template #default="{ record }">
              <a-input-number v-model:value="record.qty" :min="0" :step="1" style="width: 100%" />
            </template>
          </a-table-column>
          <a-table-column title="单价" width="160">
            <template #default="{ record }">
              <a-input-number v-model:value="record.unitPrice" :min="0" :step="0.01" style="width: 100%" />
            </template>
          </a-table-column>
          <a-table-column title="折扣" width="140">
            <template #default="{ record }">
              <a-input-number v-model:value="record.discount" :min="0" :max="1" :step="0.01" style="width: 100%" />
            </template>
          </a-table-column>
          <a-table-column title="税率" width="140">
            <template #default="{ record }">
              <a-input-number v-model:value="record.taxRate" :min="0" :max="1" :step="0.01" style="width: 100%" />
            </template>
          </a-table-column>
          <a-table-column title="交期" width="200">
            <template #default="{ record }">
              <a-date-picker v-model:value="record.deliveryDate" value-format="YYYY-MM-DD" style="width: 100%" />
            </template>
          </a-table-column>
          <a-table-column title="操作" width="120">
            <template #default="{ record }">
              <a-button type="link" danger @click="removeLine(record)">删除</a-button>
            </template>
          </a-table-column>
        </a-table>
        <div style="margin-top:8px">
          <a-button type="dashed" @click="addLine">新增行</a-button>
        </div>

        <a-divider />
        <a-space>
          <a-button type="primary" :loading="saving" @click="onSubmit">保 存</a-button>
          <a-button @click="$router.back()">返 回</a-button>
          <a-button v-if="isEdit && detail?.status==='draft'" @click="onConfirm">确 认</a-button>
          <a-button v-if="isEdit && detail?.status==='confirmed'" danger @click="onReverse">反 审</a-button>
          <a-button v-if="isEdit" @click="toPrint">打印预览</a-button>
        </a-space>
      </a-form>
    </a-card>
  </div>
</template>

<script setup lang="ts">
// 表单页面：新建/编辑订单，内置简易行编辑器，并支持确认/反审
import { onMounted, ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { message, Modal } from 'ant-design-vue';
import { getOrder, createOrder, updateOrder, confirmOrder, reverseOrder } from '@/services/orders';
import type { OrderCreateInput, OrderUpdateInput, OrderDetail, OrderLineUpdateInput } from '@/types/order';
import AccountSelect from '@/components/select/AccountSelect.vue'
import ProductSelect from '@/components/select/ProductSelect.vue'

const route = useRoute();
const router = useRouter();

const id = computed(() => route.params.id as string | undefined);
const isEdit = computed(() => !!id.value);
const pageTitle = computed(() => (isEdit.value ? '编辑订单' : '新建订单'));

const loading = ref(false);
const saving = ref(false);
const detail = ref<OrderDetail>();

// 表单模型
const form = ref<OrderCreateInput | OrderUpdateInput>({
  accountId: '',
  orderDate: undefined,
  currency: 'CNY',
  remark: '',
  lines: [],
});

function addLine() {
  (form.value.lines as OrderLineUpdateInput[]).push({
    productId: '', qty: 1, unitPrice: 0, discount: 0, taxRate: 0.13,
  });
}

function removeLine(rec: any) {
  const idx = (form.value.lines as OrderLineUpdateInput[]).indexOf(rec);
  if (idx >= 0) (form.value.lines as OrderLineUpdateInput[]).splice(idx, 1);
}

async function load() {
  if (!isEdit.value) return;
  loading.value = true;
  try {
    const data = await getOrder(id.value!);
    detail.value = data;
    form.value = {
      accountId: data.accountId,
      orderDate: data.orderDate.substring(0, 10),
      currency: data.currency,
      remark: data.remark,
      lines: data.lines.map(l => ({ id: l.id, productId: l.productId, qty: l.qty, unitPrice: l.unitPrice, discount: l.discount, taxRate: l.taxRate, deliveryDate: l.deliveryDate?.substring(0,10) }))
    } as OrderUpdateInput;
  } finally {
    loading.value = false;
  }
}

async function onSubmit() {
  // 简易校验
  if (!form.value.accountId || !form.value.lines.length) { message.error('请完善客户与至少一条明细'); return; }
  saving.value = true;
  try {
    if (isEdit.value) {
      await updateOrder(id.value!, form.value as OrderUpdateInput);
      message.success('保存成功');
    } else {
      const created = await createOrder(form.value as OrderCreateInput);
      message.success('创建成功');
      router.replace(`/orders/${created.id}`);
    }
  } finally {
    saving.value = false;
  }
}

async function onConfirm() {
  if (!id.value) return;
  await confirmOrder(id.value);
  message.success('已确认');
  await load();
}

async function onReverse() {
  if (!id.value) return;
  Modal.confirm({
    title: '反审确认',
    async onOk() {
      await reverseOrder(id.value!, {});
      message.success('已反审');
      await load();
    },
  });
}

function toPrint() {
  if (id.value) router.push(`/orders/${id.value}/print`);
}

onMounted(load);
</script>

<style scoped>
.order-form { max-width: 1280px; margin: 0 auto; }
</style>
