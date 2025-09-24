<template>
  <!-- 销售统计打印视图（中文注释）
       - 根据 URL 查询参数加载数据，并渲染为适合打印的表格
       - 支持参数：from、to、customerId、salesmanId、productId、currency、groupBy
  -->
  <div class="print-page">
    <h2>销售统计</h2>
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
          <th>订单数</th>
          <th>数量合计</th>
          <th>不含税合计</th>
          <th>税额合计</th>
          <th>含税合计</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(r, i) in rows" :key="i">
          <td>{{ r.key }}</td>
          <td>{{ r.name }}</td>
          <td class="num">{{ r.orderCount }}</td>
          <td class="num">{{ r.totalQty }}</td>
          <td class="num">{{ r.subtotal }}</td>
          <td class="num">{{ r.tax }}</td>
          <td class="num">{{ r.totalAmount }}</td>
        </tr>
      </tbody>
      <tfoot>
        <tr>
          <td colspan="3">合计</td>
          <td class="num">{{ summary.totalQty }}</td>
          <td class="num">{{ summary.subtotal }}</td>
          <td class="num">{{ summary.tax }}</td>
          <td class="num">{{ summary.totalAmount }}</td>
        </tr>
      </tfoot>
    </table>
  </div>
</template>

<script setup lang="ts">
// 说明：从路由解析参数，请求数据并渲染。
import { onMounted, reactive, ref } from 'vue'
import { useRoute } from 'vue-router'
import { fetchSalesStat } from '../../../api/modules/reports'
import type { SalesStatParams, SalesStatSummary } from '../../../types/reports'

const route = useRoute()
const rows = ref<any[]>([])
const summary = reactive({ totalQty: 0, subtotal: 0, tax: 0, totalAmount: 0 })

// 解析 URL 查询参数
const params = reactive<Record<string, any>>({ ...route.query })

function buildParams(): SalesStatParams {
  const { from, to, customerId, salesmanId, productId, currency, groupBy } = params as any
  const p: SalesStatParams = {
    customerId, salesmanId, productId, currency, groupBy,
    range: from || to ? { startUtc: from, endUtc: to } : undefined
  }
  return p
}

onMounted(async () => {
  const data: SalesStatSummary = await fetchSalesStat(buildParams())
  rows.value = data.items
  summary.totalQty = data.totalQty
  summary.subtotal = data.subtotal
  summary.tax = data.tax
  summary.totalAmount = data.totalAmount
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

