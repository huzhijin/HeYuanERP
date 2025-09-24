// CRM API：销售机会/客户拜访（最小可用）
import { http } from '../../lib/http'
import type { SalesOpportunity, CustomerVisit } from '../../types/crm'

const BASE = '/api/CRM'

// 机会列表（按状态或按用户）
export async function getOpportunitiesByStatus(status: string): Promise<SalesOpportunity[]> {
  return await http.get<SalesOpportunity[]>(`${BASE}/opportunities/by-status/${encodeURIComponent(status)}`)
}

export async function getOpportunitiesByUser(userId: number): Promise<SalesOpportunity[]> {
  return await http.get<SalesOpportunity[]>(`${BASE}/opportunities/by-user/${userId}`)
}

export async function createOpportunity(body: Partial<SalesOpportunity>): Promise<{ success: boolean; data: SalesOpportunity }> {
  return await http.post(`${BASE}/opportunities`, body)
}

export async function updateOpportunity(id: number, body: Partial<SalesOpportunity>): Promise<any> {
  return await http.put(`${BASE}/opportunities/${id}`, body)
}

export async function deleteOpportunity(id: number): Promise<any> {
  return await http.del(`${BASE}/opportunities/${id}`)
}

// 拜访列表（按客户或按用户）
export async function getVisitsByAccount(accountId: number): Promise<CustomerVisit[]> {
  return await http.get<CustomerVisit[]>(`${BASE}/visits/by-account/${accountId}`)
}

export async function getVisitsByUser(userId: number): Promise<CustomerVisit[]> {
  return await http.get<CustomerVisit[]>(`${BASE}/visits/by-user/${userId}`)
}

export async function createVisit(body: Partial<CustomerVisit>): Promise<{ success: boolean; data: CustomerVisit }> {
  return await http.post(`${BASE}/visits`, body)
}

export async function updateVisit(id: number, body: Partial<CustomerVisit>): Promise<any> {
  return await http.put(`${BASE}/visits/${id}`, body)
}

export async function deleteVisit(id: number): Promise<any> {
  return await http.del(`${BASE}/visits/${id}`)
}
