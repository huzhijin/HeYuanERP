// 采购（PO）模块前端类型定义
// 与后端 DTO 对齐，便于接口参数与返回值的类型约束

export type { Pagination } from '@/types/account';

// ========== 采购订单（PurchaseOrders） ==========

// 列表查询参数
export interface POListQuery {
  page?: number;
  size?: number;
  keyword?: string;     // 采购单号模糊
  vendorId?: string;    // 供应商过滤（AccountId）
  from?: string;        // 起始日期（ISO）
  to?: string;          // 截止日期（ISO）
  status?: 'draft' | 'confirmed';
}

// 列表项
export interface POListItem {
  id: string;
  poNo: string;
  vendorId: string;
  poDate: string;
  status: 'draft' | 'confirmed';
  remark?: string;
  totalQty: number;
  totalAmount: number;
}

// 行明细
export interface POLine {
  id: string;
  productId: string;
  qty: number;
  unitPrice: number;
}

// 详情
export interface PODetail {
  id: string;
  poNo: string;
  vendorId: string;
  poDate: string;
  status: 'draft' | 'confirmed';
  remark?: string;
  lines: POLine[];
  createdAt: string;
  createdBy?: string;
  updatedAt?: string;
  updatedBy?: string;
  totalQty: number;
  totalAmount: number;
}

// 新建/编辑输入
export interface POLineCreateInput {
  productId: string;
  qty: number;
  unitPrice: number;
}

export interface POCreateInput {
  vendorId: string;
  poDate?: string;   // ISO
  remark?: string;
  lines: POLineCreateInput[];
}

export interface POLineUpdateInput extends POLineCreateInput {
  id?: string;       // 存在则更新，不存在则新增
  _deleted?: boolean; // 标记删除
}

export interface POUpdateInput {
  vendorId: string;
  poDate?: string;
  remark?: string;
  lines: POLineUpdateInput[];
}

// ========== 收货 ==========

export interface POReceiveLineCreateInput {
  productId: string;
  qty: number;
  whse: string;
  loc?: string;
}

export interface POReceiveCreateInput {
  receiveDate?: string; // ISO
  remark?: string;
  lines: POReceiveLineCreateInput[];
}

export interface POReceiveLine {
  id: string;
  productId: string;
  qty: number;
  whse?: string;
  loc?: string;
}

export interface POReceiveDetail {
  id: string;   // 收货单 Id
  poId: string;
  receiveDate: string;
  status: string;
  remark?: string;
  lines: POReceiveLine[];
}

// ========== 导入 ==========

export interface POImportErrorItem {
  rowNo: number;
  field?: string;
  message: string;
}

export interface POImportPreviewItem {
  productId: string;
  qty: number;
  unitPrice: number;
}

export interface POImportPrecheckResult {
  vendorId: string;
  totalRecords: number;
  validRecords: number;
  invalidRecords: number;
  errors: POImportErrorItem[];
  preview: POImportPreviewItem[];
}

export interface POImportReceipt {
  vendorId: string;
  poId: string;
  poNo: string;
  createdLines: number;
  skippedLines: number;
  message: string;
}

