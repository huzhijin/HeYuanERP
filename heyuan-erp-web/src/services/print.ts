// 打印服务层：获取打印模型与 PDF 数据

import { http } from '@/utils/request';
import type { OrderPrintModel, DeliveryPrintModel } from '@/types/order';

// 打印模型预览（JSON）
export function getOrderPrintModel(id: string) {
  return http.get<OrderPrintModel>(`/api/print/order/${id}/model`);
}

export function getDeliveryPrintModel(id: string) {
  return http.get<DeliveryPrintModel>(`/api/print/delivery/${id}/model`);
}

// 获取 PDF 二进制（ArrayBuffer），调用方可用 Blob 预览/下载
export function getPrintPdf(docType: string, id: string, template = 'default') {
  return http.get<ArrayBuffer>(`/api/print/${docType}/${id}`, { params: { template }, responseType: 'arraybuffer' as any });
}

// 便捷方法：订单与送货打印
export function getOrderPdf(id: string, template = 'default') {
  return getPrintPdf('order-print', id, template);
}

export function getDeliveryPdf(id: string, template = 'default') {
  return getPrintPdf('delivery-print', id, template);
}

