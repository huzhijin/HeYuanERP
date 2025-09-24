// 说明：报表模块 TS 类型定义（中文注释）

// 通用分页结果
export interface PagedResult<T> {
  page: number
  pageSize: number
  total: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
  items: T[]
}

// 通用日期范围（UTC）
export interface DateRange {
  startUtc?: string | Date
  endUtc?: string | Date
}

// 销售统计参数
export interface SalesStatParams {
  range?: DateRange
  customerId?: string
  salesmanId?: string
  productId?: string
  currency?: string
  groupBy?: 'day' | 'month' | 'product' | 'salesman' | 'customer'
}

// 销售统计项/汇总
export interface SalesStatItem {
  key: string
  name?: string
  orderCount: number
  totalQty: number
  subtotal: number
  tax: number
  totalAmount: number
}
export interface SalesStatSummary {
  items: SalesStatItem[]
  totalQty: number
  subtotal: number
  tax: number
  totalAmount: number
}

// 发票统计参数/项/汇总
export interface InvoiceStatParams {
  range?: DateRange
  accountId?: string
  status?: string
  currency?: string
  groupBy?: 'day' | 'month' | 'account'
}
export interface InvoiceStatItem {
  key: string
  name?: string
  invoiceCount: number
  amount: number
  tax: number
  amountWithTax: number
}
export interface InvoiceStatSummary {
  items: InvoiceStatItem[]
  amount: number
  tax: number
  amountWithTax: number
}

// 采购订单查询参数/行
export interface POQueryParams {
  range?: DateRange
  vendorId?: string
  status?: string
  page?: number
  size?: number
}
export interface POListItem {
  id: string
  poNo: string
  date: string // yyyy-MM-dd
  vendorId: string
  amount: number
  status: string
}

// 库存汇总/交易与查询参数
export interface InventoryQueryParams {
  productId?: string
  whse?: string
  loc?: string
  range?: DateRange
  page?: number
  size?: number
}
export interface InventorySummary {
  productId: string
  whse: string
  loc: string
  onHand: number
  reserved: number
  available: number
}
export interface InventoryTxn {
  txnId: string
  txnCode: 'IN' | 'OUT' | 'DELIVERY' | 'RETURN' | 'PORECEIVE' | 'ADJ'
  productId: string
  qty: number
  whse: string
  loc: string
  txnDate: string // ISO 字符串
  refType?: string
  refId?: string
}

// 导出请求/任务
export type ReportName = 'sales-stat' | 'invoice-stat' | 'po-query' | 'inventory'
export type ExportFormat = 'pdf' | 'csv'
export interface ReportExportRequest {
  format?: ExportFormat
  params: Record<string, any>
}
export interface ReportTask {
  taskId: string
  status: 'queued' | 'running' | 'completed' | 'failed'
  fileUri?: string
  message?: string
  createdAt: string
  finishedAt?: string
}

