// 订单/物流 模块前端类型定义
// 与后端 DTO 对齐，便于接口参数与返回值的类型约束

// 引用通用分页类型（在 account.ts 中定义）
export type { Pagination } from '@/types/account';

// ========== 订单（SalesOrders） ==========

// 列表查询参数
export interface OrderListQuery {
  page?: number;
  size?: number;
  keyword?: string;      // 订单号模糊
  accountId?: string;    // 客户过滤
  from?: string;         // 起始日期（ISO）
  to?: string;           // 截止日期（ISO）
  status?: 'draft' | 'confirmed' | 'reversed';
}

// 列表项
export interface OrderListItem {
  id: string;
  orderNo: string;
  accountId: string;
  orderDate: string;
  currency: string;
  status: 'draft' | 'confirmed' | 'reversed';
  remark?: string;
  totalQty: number;
  totalAmount: number;
  totalTax: number;
  totalWithTax: number;
}

// 行明细
export interface OrderLine {
  id: string;
  productId: string;
  qty: number;
  unitPrice: number;
  discount: number; // 0..1
  taxRate: number;  // 0..1
  deliveryDate?: string;
}

// 详情
export interface OrderDetail {
  id: string;
  orderNo: string;
  accountId: string;
  orderDate: string;
  currency: string;
  status: 'draft' | 'confirmed' | 'reversed';
  remark?: string;
  lines: OrderLine[];
  createdAt: string;
  createdBy?: string;
  updatedAt?: string;
  updatedBy?: string;
  totalQty: number;
  totalAmount: number;
  totalTax: number;
  totalWithTax: number;
}

// 新建/编辑输入
export interface OrderLineCreateInput {
  productId: string;
  qty: number;
  unitPrice: number;
  discount: number;
  taxRate: number;
  deliveryDate?: string;
}

export interface OrderCreateInput {
  accountId: string;
  orderDate?: string; // ISO
  currency?: string;  // 默认 CNY
  remark?: string;
  lines: OrderLineCreateInput[];
}

export interface OrderLineUpdateInput extends OrderLineCreateInput {
  id?: string;       // 存在则更新，不存在则新增
  _deleted?: boolean; // 标记删除
}

export interface OrderUpdateInput {
  accountId: string;
  orderDate?: string;
  currency?: string;
  remark?: string;
  lines: OrderLineUpdateInput[];
}

export interface OrderReverseInput {
  reason?: string;
}

// ========== 送货单（Deliveries） ==========

export interface DeliveryLineCreateInput {
  productId: string;
  orderLineId?: string;
  qty: number;
}

export interface DeliveryCreateInput {
  orderId: string;
  deliveryDate?: string; // ISO
  lines: DeliveryLineCreateInput[];
}

export interface DeliveryLine {
  id: string;
  productId: string;
  orderLineId?: string;
  qty: number;
}

export interface DeliveryDetail {
  id: string;
  deliveryNo: string;
  orderId: string;
  deliveryDate: string;
  status: 'draft' | 'confirmed' | 'reversed' | string; // 预留
  lines: DeliveryLine[];
}

// 列表查询/返回（送货单）
export interface DeliveryListQuery {
  page?: number;
  size?: number;
  deliveryNo?: string; // 送货单号（模糊）
  from?: string;       // 开始日期（YYYY-MM-DD）
  to?: string;         // 结束日期（YYYY-MM-DD）
  status?: string;     // draft/confirmed/reversed
  orderNo?: string;    // 订单号（模糊）
}

export interface DeliveryListItem {
  id: string;
  deliveryNo: string;
  orderNo?: string;
  deliveryDate: string;
  status: string;
}

// ========== 退货单（Returns） ==========

export interface ReturnLineCreateInput {
  productId: string;
  qty: number;
  reason?: string;
}

export interface ReturnCreateInput {
  orderId: string;
  sourceDeliveryId?: string;
  returnDate?: string; // ISO
  lines: ReturnLineCreateInput[];
}

export interface ReturnLine {
  id: string;
  productId: string;
  qty: number;
  reason?: string;
}

export interface ReturnDetail {
  id: string;
  returnNo: string;
  orderId: string;
  sourceDeliveryId?: string;
  returnDate: string;
  status: 'draft' | 'confirmed' | 'reversed' | string;
  lines: ReturnLine[];
}

// 列表查询/返回（退货单）
export interface ReturnListQuery {
  page?: number;
  size?: number;
  returnNo?: string;        // 退货单号（模糊）
  from?: string;            // 开始日期
  to?: string;              // 结束日期
  status?: string;          // draft/confirmed/reversed
  orderNo?: string;         // 订单号（模糊）
  sourceDeliveryNo?: string;// 来源送货单号（模糊）
}

export interface ReturnListItem {
  id: string;
  returnNo: string;
  orderNo?: string;
  sourceDeliveryNo?: string;
  returnDate: string;
  status: string;
}

// ========== 打印模型（预览 JSON） ==========

export interface OrderPrintLineModel {
  id: string;
  productId: string;
  productCode: string;
  productName: string;
  spec?: string | null;
  unit?: string | null;
  qty: number;
  unitPrice: number;
  discount: number;
  taxRate: number;
  amount: number;
  tax: number;
  deliveryDate?: string;
}

export interface OrderPrintModel {
  header: {
    id: string;
    orderNo: string;
    accountId: string;
    accountName?: string;
    orderDate: string;
    currency: string;
    status: string;
    remark?: string;
  };
  lines: OrderPrintLineModel[];
  totals: {
    totalQty: number;
    totalAmount: number;
    totalTax: number;
    totalWithTax: number;
  };
}

export interface DeliveryPrintLineModel {
  id: string;
  productId: string;
  productCode: string;
  productName: string;
  spec?: string | null;
  unit?: string | null;
  orderLineId?: string;
  qty: number;
}

export interface DeliveryPrintModel {
  header: {
    id: string;
    deliveryNo: string;
    orderId: string;
    orderNo?: string;
    accountId?: string;
    accountName?: string;
    deliveryDate: string;
    status: string;
  };
  lines: DeliveryPrintLineModel[];
}
