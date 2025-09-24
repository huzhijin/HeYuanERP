// 说明：报表相关 API 封装（中文注释）
// - 与后端 OpenAPI 口径一致：统一 Envelope，导出请求 { format, params }
// - GET 查询参数：使用 from/to 或 range，服务端有白名单映射

import http, { get, post } from '../http'
import type {
  DateRange,
  InventoryQueryParams,
  InventorySummary,
  InventoryTxn,
  InvoiceStatParams,
  InvoiceStatSummary,
  POListItem,
  POQueryParams,
  PagedResult,
  ReportExportRequest,
  ReportName,
  ReportTask,
  SalesStatParams,
  SalesStatSummary
} from '../../types/reports'

// 将 DateRange 转换为 from/to（yyyy-MM-dd 或 ISO）
function rangeToQuery(range?: DateRange) {
  if (!range) return {}
  const formatDate = (v: any) => {
    if (!v) return undefined
    if (v instanceof Date) return v.toISOString().slice(0, 10) // 仅日期
    if (typeof v === 'string') {
      // 若已为 ISO，截断为日期；否则直接返回
      if (/^\d{4}-\d{2}-\d{2}/.test(v)) return v.slice(0, 10)
      return v
    }
    return v
  }
  const from = formatDate((range as any).startUtc)
  const to = formatDate((range as any).endUtc)
  return { from, to }
}

// 过滤空值，避免发送 undefined/null 到后端
function cleanParams<T extends Record<string, any>>(params: T): T {
  const out: Record<string, any> = {}
  Object.keys(params || {}).forEach((k) => {
    const v = (params as any)[k]
    if (v !== undefined && v !== null && v !== '') out[k] = v
  })
  return out as T
}

// 销售统计
export async function fetchSalesStat(params: SalesStatParams): Promise<SalesStatSummary> {
  const { range, ...rest } = params || {}
  const query = cleanParams({ ...rangeToQuery(range), ...rest })
  return await get<SalesStatSummary>('/api/reports/sales-stat', query)
}

// 发票统计
export async function fetchInvoiceStat(params: InvoiceStatParams): Promise<InvoiceStatSummary> {
  const { range, ...rest } = params || {}
  const query = cleanParams({ ...rangeToQuery(range), ...rest })
  return await get<InvoiceStatSummary>('/api/reports/invoice-stat', query)
}

// 采购订单查询（分页）
export async function fetchPOQuery(params: POQueryParams): Promise<PagedResult<POListItem>> {
  const { range, ...rest } = params || {}
  const query = cleanParams({ ...rangeToQuery(range), ...rest })
  return await get<PagedResult<POListItem>>('/api/reports/po-query', query)
}

// 库存汇总
export async function fetchInventorySummary(params: InventoryQueryParams): Promise<InventorySummary[]> {
  const { range: _ignore, ...rest } = params || {}
  const query = cleanParams({ ...rest })
  return await get<InventorySummary[]>('/api/reports/inventory/summary', query)
}

// 库存交易（分页）
export async function fetchInventoryTransactions(
  params: InventoryQueryParams
): Promise<PagedResult<InventoryTxn>> {
  const { range, ...rest } = params || {}
  const query = cleanParams({ ...rangeToQuery(range), ...rest })
  return await get<PagedResult<InventoryTxn>>('/api/reports/inventory/transactions', query)
}

// 异步导出：创建任务
export async function exportReport(name: ReportName, req: ReportExportRequest): Promise<ReportTask> {
  return await post<ReportTask>(`/api/reports/${name}/export`, req)
}

// 查询导出任务状态
export async function getReportTask(taskId: string): Promise<ReportTask> {
  return await get<ReportTask>(`/api/reports/tasks/${taskId}`)
}

// 获取快照（可用于历史记录）
export async function getSnapshotById(id: string): Promise<any> {
  return await get<any>(`/api/reports/snapshots/${id}`)
}

