<template>
  <!-- 发票统计报表视图（中文注释） -->
  <a-card title="发票统计" :bordered="false">
    <a-form layout="inline" @submit.prevent>
      <a-form-item label="日期范围">
        <a-range-picker v-model:value="dateRange" allow-clear />
      </a-form-item>
      <a-form-item label="往来户">
        <AccountSelect v-model="(form as any).accountId" placeholder="按编码/名称选择往来户" />
      </a-form-item>
      <a-form-item label="状态">
        <a-input v-model:value="form.status" placeholder="状态" allow-clear style="width: 140px" />
      </a-form-item>
      <a-form-item label="币种">
        <a-input v-model:value="form.currency" placeholder="如 CNY/USD" allow-clear style="width: 140px" />
      </a-form-item>
      <a-form-item label="分组">
        <a-select v-model:value="form.groupBy" placeholder="选择维度" style="width: 160px" allow-clear>
          <a-select-option value="day">按日</a-select-option>
          <a-select-option value="month">按月</a-select-option>
          <a-select-option value="account">按往来户</a-select-option>
        </a-select>
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

    <a-space style="margin-top: 12px">
      <a-statistic title="不含税合计" :value="summary.amount" />
      <a-statistic title="税额合计" :value="summary.tax" />
      <a-statistic title="含税合计" :value="summary.amountWithTax" />
    </a-space>

    <a-table
      style="margin-top: 12px"
      row-key="key"
      :columns="columns"
      :data-source="rows"
      :loading="loading"
      :pagination="false"
    />
  </a-card>
</template>

<script setup lang="ts">
// 说明：发票统计页面逻辑
import { reactive, ref } from 'vue'
import { message } from 'ant-design-vue'
import { fetchInvoiceStat } from '../../api/modules/reports'
import { useReportExportStore } from '../../stores/reportExport'
import type { InvoiceStatParams, InvoiceStatSummary } from '../../types/reports'
import type { Dayjs } from 'dayjs'
import AccountSelect from '@/components/select/AccountSelect.vue'

const form = reactive<Partial<InvoiceStatParams>>({
  accountId: undefined,
  status: undefined,
  currency: undefined,
  groupBy: 'day'
})

const dateRange = ref<[Dayjs, Dayjs] | null>(null)

const columns = [
  { title: '维度键', dataIndex: 'key', key: 'key' },
  { title: '名称', dataIndex: 'name', key: 'name' },
  { title: '发票数', dataIndex: 'invoiceCount', key: 'invoiceCount' },
  { title: '不含税合计', dataIndex: 'amount', key: 'amount' },
  { title: '税额合计', dataIndex: 'tax', key: 'tax' },
  { title: '含税合计', dataIndex: 'amountWithTax', key: 'amountWithTax' }
]

const rows = ref<any[]>([])
const summary = reactive({ amount: 0, tax: 0, amountWithTax: 0 })
const loading = ref(false)
const exporting = ref(false)

function buildParams(): InvoiceStatParams {
  return {
    ...form,
    range: dateRange.value
      ? { startUtc: dateRange.value[0]?.toDate(), endUtc: dateRange.value[1]?.toDate() }
      : undefined
  }
}

async function onSearch() {
  loading.value = true
  try {
    const res: InvoiceStatSummary = await fetchInvoiceStat(buildParams())
    rows.value = res.items
    summary.amount = res.amount
    summary.tax = res.tax
    summary.amountWithTax = res.amountWithTax
  } catch (e: any) {
    message.error(e?.message || '查询失败')
  } finally {
    loading.value = false
  }
}

function onReset() {
  Object.assign(form, { accountId: undefined, status: undefined, currency: undefined, groupBy: 'day' })
  dateRange.value = null
  rows.value = []
  Object.assign(summary, { amount: 0, tax: 0, amountWithTax: 0 })
}

const exportStore = useReportExportStore()
async function onExport(fmt: 'pdf' | 'csv') {
  exporting.value = true
  try {
    const taskId = await exportStore.startExport('invoice-stat', buildParams() as any, fmt)
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
