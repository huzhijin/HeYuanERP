// 库存预警前端类型（与后端 InventoryAlertsController 对齐）

export type AlertLevel = 'Info' | 'Warning' | 'Critical'
export type AlertStatus = 'Active' | 'Handled' | 'Ignored' | 'AutoResolved'
export type InventoryAlertType = 'LowStock' | 'OverStock' | 'ZeroStock'

export interface InventoryAlertInfo {
  id: string
  productId: string
  productName: string
  warehouseId?: string | null
  warehouseName?: string | null
  locationId?: string | null
  locationName?: string | null
  alertType: InventoryAlertType
  alertTypeName: string
  currentStock: number
  thresholdValue: number
  level: AlertLevel
  levelName: string
  message: string
  status: AlertStatus
  statusName: string
  createdAt: string
  handledBy?: string | null
  handledAt?: string | null
}

export interface HandleAlertRequest {
  status: AlertStatus
  remark?: string
}

export interface InventoryAlertConfigDto {
  id: string
  productId: string
  warehouseId?: string | null
  locationId?: string | null
  safetyStock: number
  maxStock: number
  isEnabled: boolean
  alertRecipients: string[]
  alertFrequencyHours: number
  lastAlertTime?: string | null
}

export interface CreateInventoryAlertConfigDto {
  productId: string
  warehouseId?: string | null
  locationId?: string | null
  safetyStock: number
  maxStock: number
  isEnabled?: boolean
  alertRecipients?: string[]
  alertFrequencyHours?: number
}

export interface UpdateInventoryAlertConfigDto {
  safetyStock: number
  maxStock: number
  isEnabled: boolean
  alertRecipients: string[]
  alertFrequencyHours: number
}

