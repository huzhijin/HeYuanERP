// 退货单服务层：创建与详情

import { http } from '@/utils/request';
import type { Pagination } from '@/types/account';
import type { ReturnCreateInput, ReturnDetail, ReturnListItem, ReturnListQuery } from '@/types/order';

const BASE = '/api/Returns';

export function createReturn(body: ReturnCreateInput) {
  return http.post<ReturnDetail>(BASE, body);
}

export function getReturn(id: string) {
  return http.get<ReturnDetail>(`${BASE}/${id}`);
}

// 列表（按单号/日期/状态，不暴露 Id）
export function listReturns(params: ReturnListQuery) {
  return http.get<Pagination<ReturnListItem>>(BASE, { params });
}
