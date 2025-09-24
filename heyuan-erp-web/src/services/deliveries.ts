// 送货单服务层：创建与详情

import { http } from '@/utils/request';
import type { Pagination } from '@/types/account';
import type { DeliveryCreateInput, DeliveryDetail, DeliveryListItem, DeliveryListQuery } from '@/types/order';

const BASE = '/api/Deliveries';

export function createDelivery(body: DeliveryCreateInput) {
  return http.post<DeliveryDetail>(BASE, body);
}

export function getDelivery(id: string) {
  return http.get<DeliveryDetail>(`${BASE}/${id}`);
}

// 列表（按单号/日期/状态，不暴露 Id）
export function listDeliveries(params: DeliveryListQuery) {
  return http.get<Pagination<DeliveryListItem>>(BASE, { params });
}
