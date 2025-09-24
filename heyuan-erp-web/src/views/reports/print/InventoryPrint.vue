<template>
  <!-- 库存报表打印视图（中文注释）
       - 根据 URL 查询参数加载“库存汇总”并渲染为打印友好表格
       - 支持参数：productId、whse、loc
  -->
  <div class="print-page">
    <h2>库存汇总</h2>
    <table class="meta">
      <tr>
        <td>仓库</td>
        <td>{{ params.whse || '-' }}</td>
      </tr>
      <tr>
        <td>库位</td>
        <td>{{ params.loc || '-' }}</td>
      </tr>
    </table>

    <table class="table">
      <thead>
        <tr>
          <th>产品</th>
          <th>仓库</th>
          <th>库位</th>
          <th>在手</th>
          <th>预留</th>
          <th>可用</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(r, i) in rows" :key="i">
          <td>{{ r.productId }}</td>
          <td>{{ r.whse }}</td>
          <td>{{ r.loc }}</td>
          <td class="num">{{ r.onHand }}</td>
          <td class="num">{{ r.reserved }}</td>
          <td class="num">{{ r.available }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
// 说明：从路由解析参数，请求数据并渲染。
import { onMounted, reactive, ref } from 'vue'
import { useRoute } from 'vue-router'
import { fetchInventorySummary } from '../../../api/modules/reports'
import type { InventoryQueryParams, InventorySummary } from '../../../types/reports'

const route = useRoute()
const params = reactive<Record<string, any>>({ ...route.query })
const rows = ref<InventorySummary[]>([])

function buildParams(): InventoryQueryParams {
  const { productId, whse, loc } = params as any
  return { productId, whse, loc }
}

onMounted(async () => {
  rows.value = await fetchInventorySummary(buildParams())
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
