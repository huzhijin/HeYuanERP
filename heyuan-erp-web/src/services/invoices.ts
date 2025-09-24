// 发票服务层封装：列表/详情/打印PDF
// 使用 utils/request 的 http 实例，自动处理统一响应与鉴权

import { http } from '@/utils/request';
import type { Pagination } from '@/types/account';

// 说明：
// - 列表/详情等接口由 InvoicesController 提供，路由前缀为 /api/Invoices（注意大小写）。
// - 打印接口由 InvoicePrintController 提供，路由为 /api/invoices/{id}/print/pdf（小写）。

const BASE = '/api/Invoices';

export type InvoiceStatus = 'Draft' | 'Pending' | 'Issued' | 'Canceled';

export interface InvoiceListQuery {
  page?: number;
  size?: number; // 前端约定，发送时映射为 pageSize
  status?: InvoiceStatus;
  customerId?: string;
}

export interface InvoiceItemDto {
  id: string;
  productCode: string;
  productName: string;
  specification?: string;
  unit?: string;
  quantity: number;
  unitPrice: number;
  taxRate: number;
  amountExcludingTax: number;
  taxAmount: number;
  amountIncludingTax: number;
  sortOrder: number;
  remark?: string;
}

export interface EInvoiceInfoDto {
  invoiceCode?: string;
  invoiceNumber?: string;
  checkCode?: string;
  pdfUrl?: string;
  viewUrl?: string;
  qrCodeUrl?: string;
  electronicIssuedAt?: string;
  buyerTaxId?: string;
  sellerTaxId?: string;
}

export interface InvoiceListItem {
  id: string;
  number: string;
  customerId: string;
  customerName: string;
  status: InvoiceStatus;
  createdAt: string;
  issuedAt?: string;
  grandTotal: number;
}

export interface InvoiceDetail extends InvoiceListItem {
  defaultTaxRate?: number;
  subtotalExcludingTax?: number;
  totalTaxAmount?: number;
  remark?: string;
  eInvoice?: EInvoiceInfoDto | null;
  items: InvoiceItemDto[];
}

export function listInvoices(params: InvoiceListQuery) {
  // 将 size → pageSize 适配为服务端参数
  const q: any = { ...params };
  if (q.size != null) {
    q.pageSize = q.size;
    delete q.size;
  }
  return http.get<any>(BASE, { params: q }).then((resp) => {
    // 兼容：若服务端被统一响应再次包装（data 内仍是 ApiResponse<T>），此处再解一层
    if (resp && resp.items && Array.isArray(resp.items)) return resp as Pagination<InvoiceListItem>;
    if (resp && typeof resp === 'object' && 'data' in resp) return (resp as any).data as Pagination<InvoiceListItem>;
    return resp as Pagination<InvoiceListItem>;
  });
}

export function getInvoice(id: string) {
  return http.get<any>(`${BASE}/${id}`).then((resp) => {
    if (resp && 'id' in resp && 'number' in resp) return resp as InvoiceDetail;
    if (resp && typeof resp === 'object' && 'data' in resp) return (resp as any).data as InvoiceDetail;
    return resp as InvoiceDetail;
  });
}

export function printInvoicePdf(id: string) {
  // 返回 PDF 二进制（ArrayBuffer），由调用方负责下载/预览
  return http.get<ArrayBuffer>(`/api/invoices/${id}/print/pdf`, { responseType: 'arraybuffer' as any });
}
