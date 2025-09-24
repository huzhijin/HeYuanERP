<template>
  <a-card title="高级报表分析" :bordered="false">
    <template #extra>
      <a-space>
        <a-date-picker v-model:value="from" valueFormat="YYYY-MM-DD" placeholder="开始日期" />
        <a-date-picker v-model:value="to" valueFormat="YYYY-MM-DD" placeholder="结束日期" />
        <a-button type="primary" @click="load">分析</a-button>
      </a-space>
    </template>

    <a-row :gutter="16">
      <a-col :span="12">
        <a-card title="产品销售 TOP（基于报价单）" :loading="loadingA">
          <a-table row-key="productId" :data-source="productSales" :pagination="false" size="small">
            <a-table-column title="产品" data-index="productName" />
            <a-table-column title="数量" data-index="totalQty" width="120" />
            <a-table-column title="金额" data-index="totalAmount" width="140" />
          </a-table>
        </a-card>
      </a-col>
      <a-col :span="12">
        <a-card title="报价单状态分布" :loading="loadingB">
          <a-table row-key="status" :data-source="quotationStats" :pagination="false" size="small">
            <a-table-column title="状态" data-index="status" />
            <a-table-column title="数量" data-index="count" width="120" />
            <a-table-column title="金额" data-index="totalAmount" width="140" />
          </a-table>
        </a-card>
      </a-col>
    </a-row>
  </a-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { fetchProductSalesStats, fetchQuotationStats } from '@/api/modules/analytics'

const from = ref<string>('')
const to = ref<string>('')
const productSales = ref<any[]>([])
const quotationStats = ref<any[]>([])
const loadingA = ref(false)
const loadingB = ref(false)

function defaultRange(){
  const d = new Date()
  const toStr = d.toISOString().slice(0,10)
  d.setMonth(d.getMonth()-1)
  const fromStr = d.toISOString().slice(0,10)
  from.value = fromStr
  to.value = toStr
}

async function load(){
  if (!from.value || !to.value) defaultRange()
  try {
    loadingA.value = true
    const a:any = await fetchProductSalesStats({ startDate: from.value, endDate: to.value })
    productSales.value = (a?.data?.items || a?.items || [])
  } catch (e:any) { message.error(e?.message || '产品销售统计失败') } finally { loadingA.value=false }

  try {
    loadingB.value = true
    const b:any = await fetchQuotationStats({ startDate: from.value, endDate: to.value })
    quotationStats.value = (b?.data?.byStatus || b?.byStatus || [])
  } catch (e:any) { message.error(e?.message || '报价单统计失败') } finally { loadingB.value=false }
}

defaultRange()
load()
</script>

<style scoped></style>

