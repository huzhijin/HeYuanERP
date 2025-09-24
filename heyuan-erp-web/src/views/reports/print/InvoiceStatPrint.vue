<template>
  <!-- 发票统计打印视图（中文注释）
       支持参数：from、to、accountId、status、currency、groupBy
  -->
  <div class="print-page">
    <h2>发票统计</h2>
    <table class="meta">
      <tr>
        <td>时间</td>
        <td>{{ params.from || '-' }} ~ {{ params.to || '-' }}</td>
      </tr>
      <tr>
        <td>分组</td>
        <td>{{ params.groupBy || 'day' }}</td>
      </tr>
    </table>

    <table class="table">
      <thead>
        <tr>
          <th>维度键</th>
          <th>名称</th>
          <th>发票数</th>
          <th>不含税合计</th>
          <th>税额合计</th>
          <th>含税合计</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(r, i) in rows" :key="i">
          <td>{{ r.key }}</td>
          <td>{{ r.name }}</td>
          <td class="num">{{ r.invoiceCount }}</td>
          <td class="num">{{ r.amount }}</td>
          <td class="num">{{ r.tax }}</td>
          <td class="num">{{ r.amountWithTax }}</td>
        </tr>
      </tbody>
      <tfoot>
        <tr>
          <td colspan="3">合计</td>
          <td class="num">{{ summary.amount }}</td>
          <td class="num">{{ summary.tax }}</td>
          <td class="num">{{ summary.amountWithTax }}</td>
        </tr>
      </tfoot>
    </table>
  </div>
</template>

<script setup lang="ts">
// 说明：从路由解析参数，请求数据并渲染。
import { onMounted, reactive, ref } from 'vue'
import { useRoute } from 'vue-router'
import { fetchInvoiceStat } from '../../../api/modules/reports'
import type { InvoiceStatParams, InvoiceStatSummary } from '../../../types/reports'

const route = useRoute()
const rows = ref<any[]>([])
const summary = reactive({ amount: 0, tax: 0, amountWithTax: 0 })

const params = reactive<Record<string, any>>({ ...route.query })

function buildParams(): InvoiceStatParams {
  const { from, to, accountId, status, currency, groupBy } = params as any
  const p: InvoiceStatParams = {
    accountId, status, currency, groupBy,
    range: from || to ? { startUtc: from, endUtc: to } : undefined
  }
  return p
}

onMounted(async () => {
  const data: InvoiceStatSummary = await fetchInvoiceStat(buildParams())
  rows.value = data.items
  summary.amount = data.amount
  summary.tax = data.tax
  summary.amountWithTax = data.amountWithTax
})
</script>

<style scoped>
.print-page { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Microsoft YaHei', sans-serif; color: #111; padding: 12px; }
table { border-collapse: collapse; width: 100%; }
table.table th, table.table td { border: 1px solid #999; padding: 6px 8px; font-size: 12px; }
table.table th { background: #f2f2f2; }
table.meta { margin-bottom: 10px; width: 60%; }
table.meta td { padding: 4px 6px; font-size: 12px; }
.num { text-align: right; }
@media print { .print-page { padding: 0; } }
</style>

