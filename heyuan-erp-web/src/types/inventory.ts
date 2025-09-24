// 库存模块前端类型定义
// - 与后端 DTO 对齐，用于接口参数与返回值的类型约束

// 引用通用分页类型（与服务端一致）
export type { Pagination } from '@/types/account';

// ========== 库存汇总 ==========

export interface InventorySummaryQuery {
  page?: number;
  size?: number;
  productId?: string;
  whse?: string;
  loc?: string;
}

export interface InventorySummaryItem {
  productId: string;
  productCode?: string;
  productName?: string;
  whse: string;
  loc?: string;
  onHand: number;
  reserved: number;
  available: number;
  avgCost?: number | null; // 报表口径占位
}

// ========== 库存事务 ==========

export interface InventoryTxnQuery {
  page?: number;
  size?: number;
  from?: string; // ISO 日期
  to?: string;   // ISO 日期
  productId?: string;
  whse?: string;
  loc?: string;
  txnCode?: 'IN' | 'OUT' | 'DELIVERY' | 'RETURN' | 'PORECEIVE' | 'ADJ' | string;
}

export interface InventoryTxnItem {
  id: string;
  txnCode: string;
  productId: string;
  productCode?: string;
  productName?: string;
  qty: number;
  whse: string;
  loc?: string;
  txnDate: string; // ISO
  refType: string;
  refId: string;
  createdAt: string; // ISO
  createdBy?: string;
}

// ========== 仓库 ==========

export interface WarehouseListQuery {
  page?: number;
  size?: number;
  keyword?: string;
  active?: boolean;
}

export interface WarehouseListItem {
  id: string;
  code: string;
  name: string;
  active: boolean;
  address?: string;
  contact?: string;
  phone?: string;
}

export interface WarehouseDetail extends WarehouseListItem {
  createdAt: string;
  createdBy?: string;
  updatedAt?: string;
  updatedBy?: string;
}

export interface WarehouseCreateInput {
  code: string;
  name: string;
  active?: boolean;
  address?: string;
  contact?: string;
  phone?: string;
}

export interface WarehouseUpdateInput extends WarehouseCreateInput {}

// ========== 库位 ==========

export interface LocationListQuery {
  page?: number;
  size?: number;
  warehouseId?: string;
  keyword?: string;
  active?: boolean;
}

export interface LocationListItem {
  id: string;
  warehouseId: string;
  code: string;
  name: string;
  active: boolean;
}

export interface LocationDetail extends LocationListItem {
  createdAt: string;
  createdBy?: string;
  updatedAt?: string;
  updatedBy?: string;
}

export interface LocationCreateInput {
  warehouseId: string;
  code: string;
  name: string;
  active?: boolean;
}

export interface LocationUpdateInput extends LocationCreateInput {}

