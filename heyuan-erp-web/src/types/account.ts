// 账户（客户）模块的前端类型定义
// 与后端 DTO 对齐，便于接口返回的类型提示与约束

// 通用分页结构（与服务端一致）
export interface Pagination<T> {
  items: T[];
  page: number;
  size: number;
  total: number;
}

// 查询参数
export interface AccountListQuery {
  page?: number;
  size?: number;
  keyword?: string;
  active?: boolean;
  ownerId?: string;
}

// 列表项
export interface AccountListItem {
  id: string;
  code: string;
  name: string;
  ownerId?: string;
  active: boolean;
  lastEventDate?: string; // ISO 字符串
  description?: string;
}

// 详情
export interface AccountDetail {
  id: string;
  code: string;
  name: string;
  ownerId?: string;
  taxNo?: string;
  active: boolean;
  lastEventDate?: string;
  description?: string;
  createdAt: string;
  createdBy?: string;
  updatedAt?: string;
  updatedBy?: string;
}

// 新建/编辑输入
export interface AccountCreateInput {
  code: string;
  name: string;
  ownerId?: string;
  taxNo?: string;
  active?: boolean;
  description?: string;
}

export interface AccountUpdateInput {
  code?: string;
  name: string;
  ownerId?: string;
  taxNo?: string;
  active?: boolean;
  description?: string;
}

// 共享/转移
export interface AccountShareRequest {
  targetUserId: string;
  permission: 'read' | 'edit';
  expireAt?: string; // ISO
}

export interface AccountTransferRequest {
  newOwnerId: string;
}

// 拜访
export interface AccountVisitCreate {
  visitDate?: string; // ISO
  contactId?: string;
  visitorId?: string;
  subject?: string;
  content?: string;
  location?: string;
  result?: string;
  nextActionAt?: string; // ISO
}

export interface AccountVisit {
  id: string;
  accountId: string;
  contactId?: string;
  visitorId?: string;
  visitDate: string;
  subject?: string;
  content?: string;
  location?: string;
  result?: string;
  nextActionAt?: string;
  createdAt: string;
  createdBy?: string;
  updatedAt?: string;
  updatedBy?: string;
}

// 附件（仅列表展示）
export interface AttachmentItem {
  id: string;
  fileName: string;
  contentType?: string;
  size: number;
  storageUri: string;
  uploadedAt: string;
  uploadedBy?: string;
}

