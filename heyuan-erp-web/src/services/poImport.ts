// 采购导入服务：预检与提交（CSV/Excel 占位）
// - 后端接口：/api/po/import/precheck 与 /api/po/import/commit（multipart/form-data）

import { http } from '@/utils/request';
import type { POImportPrecheckResult, POImportReceipt } from '@/types/purchase';

const BASE = '/api/po/import';

export function precheckPOImport(vendorId: string, file: File) {
  const fd = new FormData();
  fd.append('vendorId', vendorId);
  fd.append('file', file);
  return http.post<POImportPrecheckResult>(`${BASE}/precheck`, fd);
}

export function commitPOImport(vendorId: string, file: File) {
  const fd = new FormData();
  fd.append('vendorId', vendorId);
  fd.append('file', file);
  return http.post<POImportReceipt>(`${BASE}/commit`, fd);
}

