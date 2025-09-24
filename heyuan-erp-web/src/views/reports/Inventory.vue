<template>
  <!-- 库存报表（汇总与交易）视图（中文注释） -->
  <a-card title="库存报表" :bordered="false">
    <a-form layout="inline" @submit.prevent>
      <a-form-item label="产品">
        <ProductSelect v-model="(form as any).productId" placeholder="按编码/名称选择产品" />
      </a-form-item>
      <a-form-item label="仓库">
        <a-input v-model:value="form.whse" placeholder="仓库" allow-clear style="width: 120px" />
      </a-form-item>
      <a-form-item label="库位">
        <a-input v-model:value="form.loc" placeholder="库位" allow-clear style="width: 160px" />
      </a-form-item>
      <a-form-item label="交易日期">
        <a-range-picker v-model:value="dateRange" allow-clear />
      </a-form-item>
      <a-form-item>
        <a-space>
          <a-button type="primary" @click="onSearch" :loading="loadingSummary || loadingTxn">查询</a-button>
          <a-button @click="onReset" :disabled="loadingSummary || loadingTxn">重置</a-button>
          <a-button type="dashed" @click="onExport('pdf')" :loading="exporting">导出汇总PDF</a-button>
          <a-button type="dashed" @click="onExport('csv')" :loading="exporting">导出汇总CSV</a-button>
        </a-space>
      </a-form-item>
    </a-form>

    <a-tabs v-model:activeKey="active">
      <a-tab-pane key="summary" tab="库存汇总">
        <a-table
          row-key="productId"
          :columns="summaryColumns"
          :data-source="summaryRows"
          :loading="loadingSummary"
          :pagination="false"
        />
      </a-tab-pane>
      <a-tab-pane key="txn" tab="库存交易">
        <a-table
          row-key="txnId"
          :columns="txnColumns"
          :data-source="txnRows"
          :loading="loadingTxn"
          :pagination="false"
        />
        <div style="margin-top: 12px; text-align: right">
          <a-pagination
            :current="page"
            :pageSize="size"
            :total="total"
            @change="onPageChange"
            @showSizeChange="onSizeChange"
            :show-size-changer="true"
            :page-size-options="['10','20','50','100','200']"
          />
        </div>
      </a-tab-pane>
    </a-tabs>
  </a-card>
</template>

<script setup lang="ts">
// 说明：库存报表页面逻辑
import { reactive, ref } from 'vue'
import { message } from 'ant-design-vue'
import { fetchInventorySummary, fetchInventoryTransactions } from '../../api/modules/reports'
import { useReportExportStore } from '../../stores/reportExport'
import type { InventoryQueryParams, InventorySummary, InventoryTxn, PagedResult } from '../../types/reports'
import type { Dayjs } from 'dayjs'
import ProductSelect from '@/components/select/ProductSelect.vue'

const form = reactive<Partial<InventoryQueryParams>>({ productId: undefined, whse: undefined, loc: undefined })
const dateRange = ref<[Dayjs, Dayjs] | null>(null)
const active = ref<'summary' | 'txn'>('summary')

// 汇总列
const summaryColumns = [
  { title: '产品', dataIndex: 'productId', key: 'productId' },
  { title: '仓库', dataIndex: 'whse', key: 'whse' },
  { title: '库位', dataIndex: 'loc', key: 'loc' },
  { title: '在手', dataIndex: 'onHand', key: 'onHand' },
  { title: '预留', dataIndex: 'reserved', key: 'reserved' },
  { title: '可用', dataIndex: 'available', key: 'available' }
]

// 交易列
const txnColumns = [
  { title: '交易代码', dataIndex: 'txnCode', key: 'txnCode' },
  { title: '产品', dataIndex: 'productId', key: 'productId' },
  { title: '数量', dataIndex: 'qty', key: 'qty' },
  { title: '仓库', dataIndex: 'whse', key: 'whse' },
  { title: '库位', dataIndex: 'loc', key: 'loc' },
  { title: '时间', dataIndex: 'txnDate', key: 'txnDate' },
  { title: '来源', dataIndex: 'refType', key: 'refType' },
  // 隐藏内部 ID 列（txnId/refId）
]

const summaryRows = ref<InventorySummary[]>([])
const txnRows = ref<InventoryTxn[]>([])
const page = ref(1)
const size = ref(20)
const total = ref(0)
const loadingSummary = ref(false)
const loadingTxn = ref(false)
const exporting = ref(false)

function buildParams(): InventoryQueryParams {
  return {
    ...form,
    page: page.value,
    size: size.value,
    range: dateRange.value
      ? { startUtc: dateRange.value[0]?.toDate(), endUtc: dateRange.value[1]?.toDate() }
      : undefined
  }
}

async function loadSummary() {
  loadingSummary.value = true
  try {
    const res: InventorySummary[] = await fetchInventorySummary(buildParams())
    summaryRows.value = res
  } catch (e: any) {
    message.error(e?.message || '加载汇总失败')
  } finally {
    loadingSummary.value = false
  }
}

async function loadTxn() {
  loadingTxn.value = true
  try {
    const res: PagedResult<InventoryTxn> = await fetchInventoryTransactions(buildParams())
    txnRows.value = res.items
    page.value = res.page
    size.value = res.pageSize
    total.value = res.total
  } catch (e: any) {
    message.error(e?.message || '加载交易失败')
  } finally {
    loadingTxn.value = false
  }
}

async function onSearch() {
  page.value = 1
  await Promise.all([loadSummary(), loadTxn()])
}

function onReset() {
  Object.assign(form, { productId: undefined, whse: undefined, loc: undefined })
  dateRange.value = null
  page.value = 1
  size.value = 20
  total.value = 0
  summaryRows.value = []
  txnRows.value = []
}

function onPageChange(p: number) {
  page.value = p
  loadTxn()
}
function onSizeChange(p: number, s: number) {
  page.value = p
  size.value = s
  loadTxn()
}

const exportStore = useReportExportStore()
async function onExport(fmt: 'pdf' | 'csv') {
  exporting.value = true
  try {
    // 库存默认导出“汇总”
    const taskId = await exportStore.startExport('inventory', buildParams() as any, fmt)
    message.success(`导出任务已创建：${taskId}`)
  } catch (e: any) {
    message.error(e?.message || '创建导出任务失败')
  } finally {
    exporting.value = false
  }
}
</script>

<style scoped>
</style>
