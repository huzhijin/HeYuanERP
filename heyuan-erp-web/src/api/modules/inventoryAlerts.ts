// 库存预警 API 封装
import { http } from '../../lib/http'
import type {
  InventoryAlertInfo,
  HandleAlertRequest,
  InventoryAlertConfigDto,
  CreateInventoryAlertConfigDto,
  UpdateInventoryAlertConfigDto,
} from '../../types/inventory-alerts'

const BASE = '/api/InventoryAlerts'

export async function getActiveAlerts(): Promise<InventoryAlertInfo[]> {
  return await http.get<InventoryAlertInfo[]>(`${BASE}/active`)
}

export async function getAlertHistory(params: {
  productId?: string
  fromDate?: string
  toDate?: string
  page?: number
  size?: number
}): Promise<InventoryAlertInfo[]> {
  return await http.get<InventoryAlertInfo[]>(`${BASE}/history`, params)
}

export async function handleAlert(id: string, body: HandleAlertRequest): Promise<{ Success: boolean; Message: string }> {
  return await http.post<{ Success: boolean; Message: string }>(`${BASE}/${id}/handle`, body)
}

export async function checkNow(): Promise<any> {
  return await http.post<any>(`${BASE}/check`)
}

// 配置
export async function listConfigs(productId?: string): Promise<InventoryAlertConfigDto[]> {
  return await http.get<InventoryAlertConfigDto[]>(`${BASE}/configs`, productId ? { productId } : undefined)
}

export async function createConfig(body: CreateInventoryAlertConfigDto): Promise<InventoryAlertConfigDto> {
  return await http.post<InventoryAlertConfigDto>(`${BASE}/configs`, body)
}

export async function updateConfig(id: string, body: UpdateInventoryAlertConfigDto): Promise<InventoryAlertConfigDto> {
  return await http.put<InventoryAlertConfigDto>(`${BASE}/configs/${id}`, body)
}

export async function deleteConfig(id: string): Promise<{ Success: boolean; Message: string }> {
  return await http.del<{ Success: boolean; Message: string }>(`${BASE}/configs/${id}`)
}

export async function getStats(): Promise<any> {
  return await http.get<any>(`${BASE}/stats`)
}

export async function getTrends(): Promise<any> {
  return await http.get<any>(`${BASE}/trends`)
}
