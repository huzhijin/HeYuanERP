<template>
  <a-card title="应收账款" :bordered="false">
    <template #extra>
      <a-space>
        <a-date-picker v-model:value="from" valueFormat="YYYY-MM-DD" placeholder="开始" />
        <a-date-picker v-model:value="to" valueFormat="YYYY-MM-DD" placeholder="结束" />
        <a-input v-model:value="status" placeholder="状态（Outstanding/PartiallyPaid/FullyPaid）" style="width: 280px" />
        <a-button type="primary" @click="load">查询</a-button>
      </a-space>
    </template>
    <a-table :data-source="rows" row-key="id" :loading="loading" :pagination="false">
      <a-table-column title="单据号" data-index="documentNumber" />
      <!-- 隐藏客户ID，避免前台显示内部标识 -->
      <a-table-column title="单据日期" data-index="documentDate" width="140" />
      <a-table-column title="到期日" data-index="dueDate" width="140" />
      <a-table-column title="原金额" data-index="originalAmount" width="120" />
      <a-table-column title="已收" data-index="paidAmount" width="120" />
      <a-table-column title="余额" data-index="balanceAmount" width="120" />
      <a-table-column title="状态" data-index="status" width="140" />
    </a-table>
  </a-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { listReceivables } from '@/api/modules/finance'

const rows = ref<any[]>([])
const loading = ref(false)
const from = ref<string | null>(null)
const to = ref<string | null>(null)
const status = ref('')

async function load(){
  loading.value = true
  try{
    const p:any = {}
    if(from.value) p.startDate = from.value
    if(to.value) p.endDate = to.value
    let data:any
    if(status.value) data = await listReceivables({ status: status.value })
    else data = await listReceivables(p)
    rows.value = (data?.data || data?.items || data?.Items || data) as any[]
  }catch(e:any){ message.error(e?.message||'查询失败') } finally { loading.value=false }
}

load()
</script>

<style scoped></style>
