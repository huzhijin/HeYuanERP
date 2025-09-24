<template>
  <a-card title="库存预警" :bordered="false">
    <template #extra>
      <a-space>
        <a-button :loading="checking" @click="doCheck">手动检查</a-button>
        <a-button @click="reload">刷新</a-button>
      </a-space>
    </template>

    <a-tabs v-model:activeKey="tab">
      <a-tab-pane key="active" tab="活跃预警">
        <a-table row-key="id" :data-source="active" :loading="loading" :pagination="false">
          <a-table-column title="时间" data-index="createdAt" :customRender="r => formatDate(r.record.createdAt)" width="160" />
          <a-table-column title="产品" data-index="productName" />
          <a-table-column title="仓库/库位" :customRender="r => `${r.record.warehouseName || ''} ${r.record.locationName || ''}`" />
          <a-table-column title="类型" data-index="alertTypeName" width="120" />
          <a-table-column title="级别" data-index="levelName" width="100" />
          <a-table-column title="库存/阈值" :customRender="r => `${r.record.currentStock} / ${r.record.thresholdValue}`" width="140" />
          <a-table-column title="消息" data-index="message" />
          <a-table-column title="操作" width="200">
            <template #default="{ record }">
              <a-space>
                <a-button size="small" @click="handle(record.id, 'Handled')">标记处理</a-button>
                <a-button size="small" @click="handle(record.id, 'Ignored')">忽略</a-button>
              </a-space>
            </template>
          </a-table-column>
        </a-table>
      </a-tab-pane>
      <a-tab-pane key="history" tab="历史记录">
        <a-space style="margin-bottom: 12px">
          <a-date-picker v-model:value="fromDate" valueFormat="YYYY-MM-DD" placeholder="起始日期" />
          <a-date-picker v-model:value="toDate" valueFormat="YYYY-MM-DD" placeholder="结束日期" />
          <ProductSelect v-model="productId" placeholder="选择产品（可选）" />
          <a-button @click="loadHistory">查询</a-button>
        </a-space>
        <a-table row-key="id" :data-source="history" :loading="loadingHistory" :pagination="false">
          <a-table-column title="时间" data-index="createdAt" :customRender="r => formatDate(r.record.createdAt)" width="160" />
          <a-table-column title="产品" data-index="productName" />
          <a-table-column title="类型" data-index="alertTypeName" width="120" />
          <a-table-column title="级别" data-index="levelName" width="100" />
          <a-table-column title="状态" data-index="statusName" width="120" />
          <a-table-column title="消息" data-index="message" />
        </a-table>
      </a-tab-pane>
    </a-tabs>
  </a-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { getActiveAlerts, handleAlert, checkNow, getAlertHistory } from '@/api/modules/inventoryAlerts'
import type { InventoryAlertInfo, AlertStatus } from '@/types/inventory-alerts'
import ProductSelect from '@/components/select/ProductSelect.vue'

const tab = ref<'active' | 'history'>('active')
const active = ref<InventoryAlertInfo[]>([])
const loading = ref(false)
const checking = ref(false)

const history = ref<InventoryAlertInfo[]>([])
const loadingHistory = ref(false)
const fromDate = ref<string | null>(null)
const toDate = ref<string | null>(null)
const productId = ref<string>('')

function formatDate(v?: string) {
  if (!v) return ''
  const d = new Date(v)
  if (Number.isNaN(d.getTime())) return v
  const pad = (n: number) => (n < 10 ? `0${n}` : `${n}`)
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())} ${pad(d.getHours())}:${pad(d.getMinutes())}`
}

async function loadActive() {
  loading.value = true
  try {
    active.value = await getActiveAlerts()
  } finally {
    loading.value = false
  }
}

async function handle(id: string, status: AlertStatus) {
  try {
    await handleAlert(id, { status })
    message.success('已更新')
    await loadActive()
  } catch (e: any) {
    message.error(e?.message || '处理失败')
  }
}

async function doCheck() {
  checking.value = true
  try {
    await checkNow()
    message.success('已触发检查')
    await loadActive()
  } finally {
    checking.value = false
  }
}

async function loadHistory() {
  loadingHistory.value = true
  try {
    const p: any = {}
    if (fromDate.value) p.fromDate = fromDate.value
    if (toDate.value) p.toDate = toDate.value
    if (productId.value) p.productId = productId.value
    history.value = await getAlertHistory(p)
  } finally {
    loadingHistory.value = false
  }
}

function reload() {
  if (tab.value === 'active') loadActive()
  else loadHistory()
}

onMounted(loadActive)
</script>

<style scoped>
</style>
