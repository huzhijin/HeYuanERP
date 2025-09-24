// 采购（PO）模块接口封装：
// - 与后端 PurchaseOrdersController 对齐
// - 提供列表/详情/新增/编辑/删除/确认/收货

import { http } from '@/utils/request';
import type { Pagination } from '@/types/account';
import type {
  POListQuery,
  POListItem,
  PODetail,
  POCreateInput,
  POUpdateInput,
  POReceiveCreateInput,
  POReceiveDetail,
} from '@/types/purchase';

const BASE = '/api/PurchaseOrders';

// 列表查询
export function listPOs(params: POListQuery) {
  return http.get<Pagination<POListItem>>(BASE, { params });
}

// 详情
export function getPO(id: string) {
  return http.get<PODetail>(`${BASE}/${id}`);
}

// 新建/编辑/删除
export function createPO(body: POCreateInput) {
  return http.post<PODetail>(BASE, body);
}

export function updatePO(id: string, body: POUpdateInput) {
  return http.put<PODetail>(`${BASE}/${id}`, body);
}

export function deletePO(id: string) {
  return http.delete<void>(`${BASE}/${id}`);
}

// 确认
export function confirmPO(id: string) {
  return http.post<void>(`${BASE}/${id}/confirm`);
}

// 收货入库
export function receivePO(id: string, body: POReceiveCreateInput) {
  return http.post<POReceiveDetail>(`${BASE}/${id}/receive`, body);
}

