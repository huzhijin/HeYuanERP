// 中文说明：
// 支付（收款）相关的前端类型定义，包含统一响应与分页模型。

// 统一响应（与后端 OpenAPI 对齐）：code=0 表示成功
export interface ApiResponse<T> {
  code: number;
  message: string;
  data: T;
  traceId?: string;
}

// 分页结果
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// 列表查询参数（前端）
export interface PaymentListQuery {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  method?: string;
  minAmount?: number;
  maxAmount?: number;
  dateFrom?: string; // yyyy-MM-dd
  dateTo?: string;   // yyyy-MM-dd
  keyword?: string;
}

// 列表项 DTO
export interface PaymentListItemDto {
  id: string;
  method: string;
  amount: number;
  paymentDate: string; // yyyy-MM-dd
  remark?: string | null;
  attachmentCount: number;
}

// 附件 DTO
export interface PaymentAttachmentDto {
  id: string;
  fileName: string;
  contentType?: string | null;
  size: number;
}

// 详情 DTO
export interface PaymentDetailDto {
  id: string;
  method: string;
  amount: number;
  paymentDate: string; // yyyy-MM-dd
  remark?: string | null;
  attachments: PaymentAttachmentDto[];
}

// 创建表单（前端使用）
export interface PaymentCreateForm {
  method: string;
  amount: number;
  paymentDate: string; // yyyy-MM-dd
  remark?: string;
  attachments?: File[]; // 可选多附件
}

