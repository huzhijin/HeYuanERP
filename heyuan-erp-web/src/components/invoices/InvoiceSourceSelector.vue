<template>
  <!-- 开票来源选择器：选择来源类型（订单/交货） + 输入来源ID，支持拉取明细并回填到发票行 -->
  <a-form layout="inline">
    <a-form-item label="来源类型">
      <a-radio-group v-model:value="sourceType">
        <a-radio-button value="order">订单</a-radio-button>
        <a-radio-button value="delivery">交货</a-radio-button>
      </a-radio-group>
    </a-form-item>
    <a-form-item :label="sourceType==='order' ? '订单号' : '交货单号'" style="min-width: 360px">
      <OrderSelect v-if="sourceType==='order'" v-model="sourceId" placeholder="按订单号检索选择" />
      <DeliverySelect v-else v-model="sourceId" placeholder="按送货单号检索选择" />
    </a-form-item>
    <a-form-item>
      <a-button type="primary" @click="fetchSource" :loading="loading">拉取明细</a-button>
    </a-form-item>
  </a-form>
</template>

<script setup lang="ts">
// 来源选择器：拉取订单/交货详情并映射为发票行（商品名称/编码缺失时以 productId 占位）
import { ref } from 'vue';
import { message } from 'ant-design-vue';
import { getOrder, type OrderDetail } from '@/services/orders';
import { getDelivery, type DeliveryDetail } from '@/services/deliveries';
import OrderSelect from '@/components/select/OrderSelect.vue'
import DeliverySelect from '@/components/select/DeliverySelect.vue'

export interface InvoiceItemDraft {
  productCode: string;
  productName: string;
  specification?: string;
  unit?: string;
  quantity: number;
  unitPrice: number;
  taxRate: number;
  sortOrder: number;
  remark?: string;
}

const emits = defineEmits<{
  (e: 'resolved', payload: { sourceType: 'order' | 'delivery'; sourceId: string; sourceNumber?: string; items: InvoiceItemDraft[]; customerId?: string; customerName?: string }): void
}>();

const sourceType = ref<'order' | 'delivery'>('order');
const sourceId = ref('');
const loading = ref(false);

async function fetchSource() {
  if (!sourceId.value) { message.warning('请选择来源单据'); return; }
  loading.value = true;
  try {
    if (sourceType.value === 'order') {
      const od: OrderDetail = await getOrder(sourceId.value);
      const items: InvoiceItemDraft[] = od.lines.map((l, idx) => ({
        productCode: l.productId, // 暂以 productId 占位
        productName: l.productId,
        specification: '',
        unit: '',
        quantity: l.qty,
        unitPrice: l.unitPrice * (1 - (l.discount ?? 0)),
        taxRate: l.taxRate ?? 0.13,
        sortOrder: idx + 1,
        remark: '',
      }));
      emits('resolved', { sourceType: 'order', sourceId: od.id, sourceNumber: od.orderNo, items, customerId: od.accountId });
    } else {
      const dv: DeliveryDetail = await getDelivery(sourceId.value);
      const items: InvoiceItemDraft[] = dv.lines.map((l, idx) => ({
        productCode: l.productId,
        productName: l.productId,
        specification: '',
        unit: '',
        quantity: l.qty,
        unitPrice: 0, // 交货单无单价，需人工录入
        taxRate: 0.13,
        sortOrder: idx + 1,
        remark: '',
      }));
      emits('resolved', { sourceType: 'delivery', sourceId: dv.id, sourceNumber: dv.deliveryNo, items });
    }
    message.success('已拉取来源明细，请完善单价/税率/名称等');
  } catch (err: any) {
    message.error(err?.message || '拉取来源失败');
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
</style>
