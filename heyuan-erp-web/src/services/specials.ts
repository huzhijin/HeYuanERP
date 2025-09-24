// 特价/项目申请（留桩）服务层：提交申请与查询状态

import { http } from '@/utils/request';

export interface SpecialLineInput {
  productId: string;
  targetUnitPrice: number;
  remark?: string;
}

export interface SpecialApplyInput {
  orderId: string;
  projectName: string;
  reason?: string;
  lines: SpecialLineInput[];
}

export interface SpecialApplyResult {
  applyId: string;
  status: string;
  message: string;
  echo?: any;
}

export function applySpecial(body: SpecialApplyInput) {
  return http.post<SpecialApplyResult>('/api/Orders/specials/apply', body);
}

export function getSpecialStatus(applyId: string) {
  return http.get<SpecialApplyResult>(`/api/Orders/specials/${applyId}`);
}

