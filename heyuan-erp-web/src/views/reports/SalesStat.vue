<template>
  <!-- 销售统计报表视图（中文注释） -->
  <a-card title="销售统计" :bordered="false">
    <a-form layout="inline" @submit.prevent>
      <a-form-item label="日期范围">
        <a-range-picker v-model:value="dateRange" allow-clear />
      </a-form-item>
      <a-form-item label="客户">
        <AccountSelect v-model="(form as any).customerId" placeholder="按编码/名称选择客户" />
      </a-form-item>
      <a-form-item label="业务员">
        <a-input v-model:value="form.salesmanId" placeholder="业务员" allow-clear style="width: 180px" />
      </a-form-item>
      <a-form-item label="产品">
        <ProductSelect v-model="(form as any).productId" placeholder="按编码/名称选择产品" />
      </a-form-item>
      <a-form-item label="币种">
        <a-input v-model:value="form.currency" placeholder="如 CNY/USD" allow-clear style="width: 140px" />
      </a-form-item>
      <a-form-item label="分组">
        <a-select v-model:value="form.groupBy" placeholder="选择维度" style="width: 160px" allow-clear>
          <a-select-option value="day">按日</a-select-option>
          <a-select-option value="month">按月</a-select-option>
          <a-select-option value="product">按产品</a-select-option>
          <a-select-option value="salesman">按业务员</a-select-option>
          <a-select-option value="customer">按客户</a-select-option>
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
      <a-statistic title="数量合计" :value="summary.totalQty" />
      <a-statistic title="不含税合计" :value="summary.subtotal" />
      <a-statistic title="税额合计" :value="summary.tax" />
      <a-statistic title="含税合计" :value="summary.totalAmount" />
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
// 说明：销售统计页面逻辑
import { reactive, ref } from 'vue'
import { message } from 'ant-design-vue'
import { fetchSalesStat } from '../../api/modules/reports'
import { useReportExportStore } from '../../stores/reportExport'
import type { SalesStatParams, SalesStatSummary } from '../../types/reports'
import type { Dayjs } from 'dayjs'
import AccountSelect from '@/components/select/AccountSelect.vue'
import ProductSelect from '@/components/select/ProductSelect.vue'

// 查询表单状态
const form = reactive<Partial<SalesStatParams>>({
  customerId: undefined,
  salesmanId: undefined,
  productId: undefined,
  currency: undefined,
  groupBy: 'day'
})

// 日期范围（RangePicker 使用 dayjs）
const dateRange = ref<[Dayjs, Dayjs] | null>(null)

// 表格列定义
const columns = [
  { title: '维度键', dataIndex: 'key', key: 'key' },
  { title: '名称', dataIndex: 'name', key: 'name' },
  { title: '订单数', dataIndex: 'orderCount', key: 'orderCount' },
  { title: '数量合计', dataIndex: 'totalQty', key: 'totalQty' },
  { title: '不含税合计', dataIndex: 'subtotal', key: 'subtotal' },
  { title: '税额合计', dataIndex: 'tax', key: 'tax' },
  { title: '含税合计', dataIndex: 'totalAmount', key: 'totalAmount' }
]

// 数据与状态
const rows = ref<any[]>([])
const summary = reactive({ totalQty: 0, subtotal: 0, tax: 0, totalAmount: 0 })
const loading = ref(false)
const exporting = ref(false)

// 构造查询参数
function buildParams(): SalesStatParams {
  const params: SalesStatParams = {
    ...form,
    range: dateRange.value
      ? { startUtc: dateRange.value[0]?.toDate(), endUtc: dateRange.value[1]?.toDate() }
      : undefined
  }
  return params
}

// 查询
async function onSearch() {
  loading.value = true
  try {
    const res: SalesStatSummary = await fetchSalesStat(buildParams())
    rows.value = res.items
    summary.totalQty = res.totalQty
    summary.subtotal = res.subtotal
    summary.tax = res.tax
    summary.totalAmount = res.totalAmount
  } catch (e: any) {
    message.error(e?.message || '查询失败')
  } finally {
    loading.value = false
  }
}

// 重置
function onReset() {
  Object.assign(form, { customerId: undefined, salesmanId: undefined, productId: undefined, currency: undefined, groupBy: 'day' })
  dateRange.value = null
  rows.value = []
  Object.assign(summary, { totalQty: 0, subtotal: 0, tax: 0, totalAmount: 0 })
}

// 导出
const exportStore = useReportExportStore()
async function onExport(fmt: 'pdf' | 'csv') {
  exporting.value = true
  try {
    const taskId = await exportStore.startExport('sales-stat', buildParams() as any, fmt)
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
