// 中文说明：
// 支付/对账相关路由模块。

import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/payments',
    name: 'PaymentsList',
    meta: { title: '收款列表' },
    component: () => import('../../pages/payments/PaymentList.vue'),
  },
  {
    path: '/payments/create',
    name: 'PaymentCreate',
    meta: { title: '新增收款' },
    component: () => import('../../pages/payments/PaymentCreate.vue'),
  },
  {
    path: '/reconciliation/export',
    name: 'ReconciliationExport',
    meta: { title: '对账单导出' },
    component: () => import('../../pages/reconciliation/ReconciliationExport.vue'),
  },
];

export default routes;

