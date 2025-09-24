// 库存服务层封装：库存汇总与库存事务
// 使用 utils/request 的 http 实例，自动处理统一响应与鉴权

import { http } from '@/utils/request';
import type { Pagination } from '@/types/account';
import type { InventorySummaryQuery, InventorySummaryItem, InventoryTxnQuery, InventoryTxnItem } from '@/types/inventory';

const BASE = '/api/inventory';

// 库存汇总
export function listInventorySummary(params: InventorySummaryQuery) {
  return http.get<Pagination<InventorySummaryItem>>(`${BASE}/summary`, { params });
}

// 库存事务
export function listInventoryTransactions(params: InventoryTxnQuery) {
  return http.get<Pagination<InventoryTxnItem>>(`${BASE}/transactions`, { params });
}

