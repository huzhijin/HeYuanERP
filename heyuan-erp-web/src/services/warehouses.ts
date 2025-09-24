// 仓库服务层封装：CRUD
// 与后端 WarehousesController 对齐

import { http } from '@/utils/request';
import type { Pagination } from '@/types/account';
import type { WarehouseListQuery, WarehouseListItem, WarehouseDetail, WarehouseCreateInput, WarehouseUpdateInput } from '@/types/inventory';

const BASE = '/api/Warehouses';

export function listWarehouses(params: WarehouseListQuery) {
  return http.get<Pagination<WarehouseListItem>>(BASE, { params });
}

export function getWarehouse(id: string) {
  return http.get<WarehouseDetail>(`${BASE}/${id}`);
}

export function createWarehouse(body: WarehouseCreateInput) {
  return http.post<WarehouseDetail>(BASE, body);
}

export function updateWarehouse(id: string, body: WarehouseUpdateInput) {
  return http.put<WarehouseDetail>(`${BASE}/${id}`, body);
}

export function deleteWarehouse(id: string) {
  return http.delete<void>(`${BASE}/${id}`);
}

