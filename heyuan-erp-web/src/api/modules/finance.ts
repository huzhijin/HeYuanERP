// 财务 AR/AP 与仪表板 API（最小可用）
import { http } from '../../lib/http'

// 应收列表
export async function listReceivables(params: any): Promise<any> {
  return await http.get('/api/AccountsReceivablePayable/receivables', params)
}

// 应付列表
export async function listPayables(params: any): Promise<any> {
  return await http.get('/api/AccountsReceivablePayable/payables', params)
}

// 账龄/仪表板
export async function getReceivableDashboard(): Promise<any> {
  return await http.get('/api/AccountsReceivablePayable/dashboard/receivable')
}

export async function getPayableDashboard(): Promise<any> {
  return await http.get('/api/AccountsReceivablePayable/dashboard/payable')
}

export async function getCashFlowDashboard(): Promise<any> {
  return await http.get('/api/AccountsReceivablePayable/dashboard/cash-flow')
}

