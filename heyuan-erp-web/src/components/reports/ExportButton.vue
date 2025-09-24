<template>
  <!-- 通用导出按钮（中文注释）
       props：
       - report：报表名称（sales-stat|invoice-stat|po-query|inventory）
       - params：导出参数对象（与后端白名单对应）
       - mode：展示模式（single 显示一个按钮；menu 显示下拉菜单）
       - defaultFormat：默认格式 pdf/csv
  -->
  <div>
    <a-button v-if="mode === 'single'" type="dashed" :loading="loading" @click="doExport(defaultFormat)">
      导出{{ defaultFormat.toUpperCase() }}
    </a-button>
    <a-dropdown v-else>
      <a-button type="dashed" :loading="loading">
        导出 <DownOutlined />
      </a-button>
      <template #overlay>
        <a-menu @click="onMenuClick">
          <a-menu-item key="pdf">导出 PDF</a-menu-item>
          <a-menu-item key="csv">导出 CSV</a-menu-item>
        </a-menu>
      </template>
    </a-dropdown>
  </div>
</template>

<script setup lang="ts">
// 说明：封装导出调用与状态
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { DownOutlined } from '@ant-design/icons-vue'
import { useReportExportStore } from '../../stores/reportExport'
import type { ExportFormat, ReportName } from '../../types/reports'

const props = defineProps<{
  report: ReportName
  params: Record<string, any>
  mode?: 'single' | 'menu'
  defaultFormat?: ExportFormat
}>()

const emit = defineEmits<{ (e: 'started', taskId: string): void }>()
const loading = ref(false)
const store = useReportExportStore()

async function doExport(fmt: ExportFormat) {
  loading.value = true
  try {
    const taskId = await store.startExport(props.report, props.params, fmt)
    emit('started', taskId)
    message.success(`导出任务已创建：${taskId}`)
  } catch (e: any) {
    message.error(e?.message || '创建导出任务失败')
  } finally {
    loading.value = false
  }
}

function onMenuClick({ key }: any) {
  doExport(key as ExportFormat)
}

// 默认值
const mode = props.mode ?? 'menu'
const defaultFormat = (props.defaultFormat ?? 'pdf') as ExportFormat
</script>

<style scoped>
</style>

