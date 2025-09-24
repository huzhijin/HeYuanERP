<template>
  <!-- 采购订单查询（分页）视图（中文注释） -->
  <a-card title="采购订单查询" :bordered="false">
    <a-form layout="inline" @submit.prevent>
        <a-form-item label="日期范围">
          <a-range-picker v-model:value="dateRange" allow-clear />
        </a-form-item>
        <!-- 避免通过供应商ID筛选，此处不提供供应商筛选输入 -->
      <a-form-item label="状态">
        <a-input v-model:value="form.status" placeholder="状态" allow-clear style="width: 140px" />
      </a-form-item>
      <a-form-item>
        <a-space>
          <a-button type="primary" @click="onSearch" :loading="loading">查询</a-button>
          <a-button @click="onReset" :disabled="loading">重置</a-button>
          <a-button type="dashed" @click="onExport('pdf')" :loading="exporting">导出PDF</a-button>
          <a-button type="dashed" @click="onExport('csv')" :loading="exporting">导出CSV</a-button>
        </a-space>
      </a-form-item>
    </a-form>

    <a-table
      style="margin-top: 12px"
      row-key="id"
      :columns="columns"
      :data-source="rows"
      :loading="loading"
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
  </a-card>
</template>

<script setup lang="ts">
// 说明：采购订单查询页面逻辑（分页）
import { reactive, ref } from 'vue'
import { message } from 'ant-design-vue'
import { fetchPOQuery } from '../../api/modules/reports'
import { useReportExportStore } from '../../stores/reportExport'
import type { POQueryParams, PagedResult, POListItem } from '../../types/reports'
import type { Dayjs } from 'dayjs'

const form = reactive<Partial<POQueryParams>>({ vendorId: undefined, status: undefined })
const dateRange = ref<[Dayjs, Dayjs] | null>(null)

const columns = [
  { title: '采购单号', dataIndex: 'poNo', key: 'poNo' },
  { title: '业务日期', dataIndex: 'date', key: 'date' },
  // 不显示供应商ID列，避免前台展示内部标识
  { title: '金额', dataIndex: 'amount', key: 'amount' },
  { title: '状态', dataIndex: 'status', key: 'status' }
]

const rows = ref<POListItem[]>([])
const page = ref(1)
const size = ref(20)
const total = ref(0)
const loading = ref(false)
const exporting = ref(false)

function buildParams(): POQueryParams {
  return {
    ...form,
    page: page.value,
    size: size.value,
    range: dateRange.value
      ? { startUtc: dateRange.value[0]?.toDate(), endUtc: dateRange.value[1]?.toDate() }
      : undefined
  }
}

async function load() {
  loading.value = true
  try {
    const res: PagedResult<POListItem> = await fetchPOQuery(buildParams())
    rows.value = res.items
    page.value = res.page
    size.value = res.pageSize
    total.value = res.total
  } catch (e: any) {
    message.error(e?.message || '查询失败')
  } finally {
    loading.value = false
  }
}

async function onSearch() {
  page.value = 1
  await load()
}

function onReset() {
  Object.assign(form, { vendorId: undefined, status: undefined })
  dateRange.value = null
  page.value = 1
  size.value = 20
  rows.value = []
  total.value = 0
}

function onPageChange(p: number) {
  page.value = p
  load()
}
function onSizeChange(p: number, s: number) {
  page.value = p
  size.value = s
  load()
}

const exportStore = useReportExportStore()
async function onExport(fmt: 'pdf' | 'csv') {
  exporting.value = true
  try {
    const taskId = await exportStore.startExport('po-query', buildParams() as any, fmt)
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
