// 说明：报表导出任务 Pinia Store（中文注释）
// - 负责创建导出任务、轮询任务状态、暴露下载地址

import { defineStore } from 'pinia'
import { exportReport, getReportTask } from '../api/modules/reports'
import type { ExportFormat, ReportName, ReportTask } from '../types/reports'

interface TaskState extends ReportTask {
  // 本地辅助字段：下次轮询时间
  _timer?: any
}

interface State {
  tasks: Record<string, TaskState>
}

export const useReportExportStore = defineStore('reportExport', {
  state: (): State => ({ tasks: {} }),
  getters: {
    // 根据任务 Id 获取任务
    getTask: (s) => (id: string) => s.tasks[id]
  },
  actions: {
    // 创建导出任务，并开始轮询
    async startExport(name: ReportName, params: Record<string, any>, format: ExportFormat = 'pdf') {
      const task = await exportReport(name, { format, params })
      this.tasks[task.taskId] = { ...task }
      this.pollTask(task.taskId)
      return task.taskId
    },

    // 轮询任务状态，直至完成或失败
    async pollTask(taskId: string) {
      const cur = this.tasks[taskId]
      if (!cur) return
      if (cur.status === 'completed' || cur.status === 'failed') return
      try {
        const updated = await getReportTask(taskId)
        this.tasks[taskId] = { ...updated }
        if (updated.status === 'queued' || updated.status === 'running') {
          // 指数退避或固定间隔，这里采用固定 1.5s
          this.tasks[taskId]._timer = setTimeout(() => this.pollTask(taskId), 1500)
        }
      } catch (e) {
        // 错误时稍后再试，避免死循环
        this.tasks[taskId]._timer = setTimeout(() => this.pollTask(taskId), 3000)
      }
    },

    // 停止轮询并清理
    stop(taskId: string) {
      const cur = this.tasks[taskId]
      if (cur && cur._timer) {
        clearTimeout(cur._timer)
        delete cur._timer
      }
    },

    // 清除任务记录
    clear(taskId?: string) {
      if (taskId) {
        this.stop(taskId)
        delete this.tasks[taskId]
      } else {
        Object.keys(this.tasks).forEach((id) => this.stop(id))
        this.tasks = {}
      }
    }
  }
})

