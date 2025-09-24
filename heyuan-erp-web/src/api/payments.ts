// 中文说明：
// 收款与对账相关 API 封装（手写 axios，不生成 SDK）。

import http from '../lib/http';
import type { PagedResult, PaymentListItemDto, PaymentListQuery, PaymentCreateForm, PaymentDetailDto } from '../types/payments';

// 将 yyyy-MM-dd 字符串安全化（避免空值）
function normalizeDateStr(d?: string): string | undefined {
  if (!d) return undefined;
  return d.trim();
}

// 列表查询
export async function listPayments(params: PaymentListQuery): Promise<PagedResult<PaymentListItemDto>> {
  const query = {
    page: params.page ?? 1,
    pageSize: params.pageSize ?? 20,
    sortBy: params.sortBy ?? 'paymentDate',
    sortOrder: params.sortOrder ?? 'desc',
    method: params.method || undefined,
    minAmount: params.minAmount,
    maxAmount: params.maxAmount,
    dateFrom: normalizeDateStr(params.dateFrom),
    dateTo: normalizeDateStr(params.dateTo),
    keyword: params.keyword || undefined,
  } as any;
  return await http.get<PagedResult<PaymentListItemDto>>('/api/payments', query);
}

// 创建收款（multipart/form-data）
export async function createPayment(form: PaymentCreateForm): Promise<PaymentDetailDto> {
  const fd = new FormData();
  // 注意：为兼容后端 .NET 绑定，字段名使用首字母大写
  fd.append('Method', form.method);
  fd.append('Amount', String(form.amount));
  fd.append('PaymentDate', form.paymentDate);
  if (form.remark) fd.append('Remark', form.remark);
  if (form.attachments && form.attachments.length > 0) {
    for (const f of form.attachments) fd.append('Attachments', f, f.name);
  }
  return await http.postForm<PaymentDetailDto>('/api/payments', fd);
}

// 获取详情
export async function getPayment(id: string): Promise<PaymentDetailDto> {
  return await http.get<PaymentDetailDto>(`/api/payments/${id}`);
}

// 对账单导出（CSV），返回 blob 与文件名
export async function exportReconciliation(params: { dateFrom?: string; dateTo?: string; method?: string }): Promise<{ blob: Blob; fileName: string }> {
  const { blob, fileName } = await http.getBlob('/api/reconciliation/export', {
    dateFrom: normalizeDateStr(params.dateFrom),
    dateTo: normalizeDateStr(params.dateTo),
    method: params.method || undefined,
  });
  return { blob, fileName: fileName || `reconciliation_${Date.now()}.csv` };
}

