// 账户（客户）模块的接口封装：
// - 统一使用 utils/request.ts 的 http 实例，自动附带鉴权与错误处理
// - 与 OpenAPI/后端 DTO 对齐，返回强类型结果

import { http } from '@/utils/request';
import type {
  Pagination,
  AccountListQuery,
  AccountListItem,
  AccountDetail,
  AccountCreateInput,
  AccountUpdateInput,
  AccountShareRequest,
  AccountTransferRequest,
  AccountVisitCreate,
  AccountVisit,
  AttachmentItem,
} from '@/types/account';

const BASE = '/api/Accounts';

// 列表查询
export function listAccounts(params: AccountListQuery) {
  return http.get<Pagination<AccountListItem>>(BASE, { params });
}

// 详情
export function getAccount(id: string) {
  return http.get<AccountDetail>(`${BASE}/${id}`);
}

// 编码唯一性预校验
export function existsCode(code: string, excludeId?: string) {
  return http.get<{ exists: boolean }>(`${BASE}/exists-code`, {
    params: { code, excludeId },
  });
}

// 新建/编辑
export function createAccount(body: AccountCreateInput) {
  return http.post<AccountDetail>(BASE, body);
}

export function updateAccount(id: string, body: AccountUpdateInput) {
  return http.put<AccountDetail>(`${BASE}/${id}`, body);
}

// 共享/取消共享/转移
export function shareAccount(id: string, body: AccountShareRequest) {
  return http.post<void>(`${BASE}/${id}/share`, body);
}

export function unshareAccount(id: string, targetUserId: string) {
  return http.delete<void>(`${BASE}/${id}/share/${targetUserId}`);
}

export function transferAccount(id: string, body: AccountTransferRequest) {
  return http.post<void>(`${BASE}/${id}/transfer`, body);
}

// 拜访
export function listAccountVisits(id: string, page = 1, size = 20) {
  return http.get<Pagination<AccountVisit>>(`${BASE}/${id}/visits`, { params: { page, size } });
}

export function createAccountVisit(id: string, body: AccountVisitCreate) {
  return http.post<AccountVisit>(`${BASE}/${id}/visits`, body);
}

// 附件列表（仅查看）
export function listAccountAttachments(id: string) {
  return http.get<AttachmentItem[]>(`${BASE}/${id}/attachments`);
}

