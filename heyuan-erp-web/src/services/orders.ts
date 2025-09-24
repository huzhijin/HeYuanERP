// 订单服务层封装：CRUD + 确认/反审
// 使用 utils/request 的 http 实例，自动处理统一响应与鉴权

import { http } from '@/utils/request';
import type { Pagination } from '@/types/account';
import type {
  OrderListQuery,
  OrderListItem,
  OrderDetail,
  OrderCreateInput,
  OrderUpdateInput,
  OrderReverseInput,
} from '@/types/order';

const BASE = '/api/Orders';

export function listOrders(params: OrderListQuery) {
  return http.get<Pagination<OrderListItem>>(BASE, { params });
}

export function getOrder(id: string) {
  return http.get<OrderDetail>(`${BASE}/${id}`);
}

export function createOrder(body: OrderCreateInput) {
  return http.post<OrderDetail>(BASE, body);
}

export function updateOrder(id: string, body: OrderUpdateInput) {
  return http.put<OrderDetail>(`${BASE}/${id}`, body);
}

export function deleteOrder(id: string) {
  return http.delete<void>(`${BASE}/${id}`);
}

export function confirmOrder(id: string) {
  return http.post<void>(`${BASE}/${id}/confirm`);
}

export function reverseOrder(id: string, body: OrderReverseInput) {
  return http.post<void>(`${BASE}/${id}/reverse`, body);
}

