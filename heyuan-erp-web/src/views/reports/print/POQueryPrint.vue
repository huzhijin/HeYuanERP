<template>
  <!-- 采购订单查询打印视图（中文注释）
       支持参数：from、to、vendorId、status、size（默认 200）
  -->
  <div class="print-page">
    <h2>采购订单查询</h2>
    <table class="meta">
      <tr>
        <td>时间</td>
        <td>{{ params.from || '-' }} ~ {{ params.to || '-' }}</td>
      </tr>
      <tr>
        <td>状态</td>
        <td>{{ params.status || '-' }}</td>
      </tr>
    </table>

    <table class="table">
      <thead>
        <tr>
          <th>采购单号</th>
          <th>业务日期</th>
          <!-- 不输出供应商内部ID -->
          <th>金额</th>
          <th>状态</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(r, i) in rows" :key="i">
          <td>{{ r.poNo }}</td>
          <td>{{ r.date }}</td>
          <!-- 供应商列移除，避免输出内部ID -->
          <td class="num">{{ r.amount }}</td>
          <td>{{ r.status }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
// 说明：从路由解析参数，请求数据并渲染。
import { onMounted, reactive, ref } from 'vue'
import { useRoute } from 'vue-router'
import { fetchPOQuery } from '../../../api/modules/reports'
import type { POListItem, POQueryParams, PagedResult } from '../../../types/reports'

const route = useRoute()
const rows = ref<POListItem[]>([])
const params = reactive<Record<string, any>>({ ...route.query })

function buildParams(): POQueryParams {
  const { from, to, vendorId, status } = params as any
  const size = Number((params as any).size || 200)
  const p: POQueryParams = {
    vendorId, status, page: 1, size,
    range: from || to ? { startUtc: from, endUtc: to } : undefined
  }
  return p
}

onMounted(async () => {
  const data: PagedResult<POListItem> = await fetchPOQuery(buildParams())
  rows.value = data.items
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
