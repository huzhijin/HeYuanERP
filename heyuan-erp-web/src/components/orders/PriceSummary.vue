<template>
  <!-- 订单金额汇总：展示不含税/税额/含税合计与数量合计 -->
  <a-descriptions size="small" :column="4" bordered>
    <a-descriptions-item label="总数量">{{ totals.totalQty.toFixed(4) }}</a-descriptions-item>
    <a-descriptions-item label="不含税金额">{{ totals.totalAmount.toFixed(2) }}</a-descriptions-item>
    <a-descriptions-item label="税额">{{ totals.totalTax.toFixed(2) }}</a-descriptions-item>
    <a-descriptions-item label="含税金额">{{ totals.totalWithTax.toFixed(2) }}</a-descriptions-item>
  </a-descriptions>
</template>

<script setup lang="ts">
// 汇总展示组件：根据订单行计算合计；与后端 PricingService 口径一致
import { computed } from 'vue';

type Line = { qty: number; unitPrice: number; discount: number; taxRate: number };
const props = defineProps<{ lines: Line[] }>();

const totals = computed(() => {
  let totalQty = 0, totalAmount = 0, totalTax = 0;
  for (const l of props.lines || []) {
    const price = l.unitPrice * (1 - l.discount);
    const amount = l.qty * price;
    const tax = amount * l.taxRate;
    totalQty += l.qty;
    totalAmount += Math.round(amount * 100) / 100;
    totalTax += Math.round(tax * 100) / 100;
  }
  const totalWithTax = totalAmount + totalTax;
  return { totalQty, totalAmount, totalTax, totalWithTax };
});
</script>

<style scoped>
</style>

