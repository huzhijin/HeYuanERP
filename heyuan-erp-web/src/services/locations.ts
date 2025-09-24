// 库位服务层封装：CRUD
// 与后端 LocationsController 对齐

import { http } from '@/utils/request';
import type { Pagination } from '@/types/account';
import type { LocationListQuery, LocationListItem, LocationDetail, LocationCreateInput, LocationUpdateInput } from '@/types/inventory';

const BASE = '/api/Locations';

export function listLocations(params: LocationListQuery) {
  return http.get<Pagination<LocationListItem>>(BASE, { params });
}

export function getLocation(id: string) {
  return http.get<LocationDetail>(`${BASE}/${id}`);
}

export function createLocation(body: LocationCreateInput) {
  return http.post<LocationDetail>(BASE, body);
}

export function updateLocation(id: string, body: LocationUpdateInput) {
  return http.put<LocationDetail>(`${BASE}/${id}`, body);
}

export function deleteLocation(id: string) {
  return http.delete<void>(`${BASE}/${id}`);
}

