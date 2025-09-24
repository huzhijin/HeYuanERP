<template>
  <!-- 发票详情页：展示发票头与行，提供打印按钮 -->
  <div class="page invoice-detail">
    <a-page-header :title="`发票详情：${invoice?.number || ''}`" @back="goBack">
      <template #extra>
        <InvoicePrintButton v-if="id" :id="id" />
      </template>
    </a-page-header>

    <a-card :bordered="false" style="margin-bottom: 12px">
      <a-descriptions title="基本信息" bordered size="small" :column="2">
        <a-descriptions-item label="发票号">{{ invoice?.number }}</a-descriptions-item>
        <a-descriptions-item label="客户">{{ invoice?.customerName }} ({{ invoice?.customerId }})</a-descriptions-item>
        <a-descriptions-item label="状态"><InvoiceStatusTag :status="invoice?.status as any" /></a-descriptions-item>
        <a-descriptions-item label="创建时间">{{ fmt(invoice?.createdAt) }}</a-descriptions-item>
        <a-descriptions-item label="开具时间">{{ fmt(invoice?.issuedAt) }}</a-descriptions-item>
        <a-descriptions-item label="含税合计">{{ n2(invoice?.grandTotal) }}</a-descriptions-item>
      </a-descriptions>
    </a-card>

    <a-card :bordered="false" title="明细列表">
      <a-table :data-source="invoice?.items || []" :pagination="false" row-key="id">
        <a-table-column title="#" width="60">
          <template #default="{ index }">{{ index + 1 }}</template>
        </a-table-column>
        <a-table-column key="productCode" data-index="productCode" title="商品编码" width="160" />
        <a-table-column key="productName" data-index="productName" title="商品名称" width="200" />
        <a-table-column key="specification" data-index="specification" title="规格" width="140" />
        <a-table-column key="unit" data-index="unit" title="单位" width="100" />
        <a-table-column key="quantity" data-index="quantity" title="数量" width="120" />
        <a-table-column key="unitPrice" data-index="unitPrice" title="单价" width="120">
          <template #default="{ text }">{{ n2(text) }}</template>
        </a-table-column>
        <a-table-column key="taxRate" data-index="taxRate" title="税率" width="100">
          <template #default="{ text }">{{ (Number(text) * 100).toFixed(2) }}%</template>
        </a-table-column>
        <a-table-column key="amountExcludingTax" data-index="amountExcludingTax" title="不含税金额" width="140">
          <template #default="{ text }">{{ n2(text) }}</template>
        </a-table-column>
        <a-table-column key="taxAmount" data-index="taxAmount" title="税额" width="120">
          <template #default="{ text }">{{ n2(text) }}</template>
        </a-table-column>
        <a-table-column key="amountIncludingTax" data-index="amountIncludingTax" title="含税金额" width="140">
          <template #default="{ text }">{{ n2(text) }}</template>
        </a-table-column>
        <a-table-column key="remark" data-index="remark" title="备注" />
      </a-table>
    </a-card>
  </div>
</template>

<script setup lang="ts">
// 发票详情页
import dayjs from 'dayjs';
import { onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { getInvoice, type InvoiceDetail } from '@/services/invoices';
import InvoiceStatusTag from '@/components/invoices/InvoiceStatusTag.vue';
import InvoicePrintButton from '@/components/invoices/InvoicePrintButton.vue';

const route = useRoute();
const router = useRouter();
const id = route.params.id as string | undefined;
const invoice = ref<InvoiceDetail | null>(null);

function fmt(s?: string) { return s ? dayjs(s).format('YYYY-MM-DD HH:mm') : '-'; }
function n2(x: any) { const n = Number(x ?? 0); return n.toFixed(2); }
function goBack() { router.back(); }

async function load() {
  if (!id) return;
  const data = await getInvoice(id);
  invoice.value = data;
}

onMounted(load);
</script>

<style scoped>
</style>

