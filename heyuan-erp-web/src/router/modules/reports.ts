// 说明：报表模块路由配置（中文注释）
// - 仅声明路由，视图组件在后续批次提供

import type { RouteRecordRaw } from 'vue-router'

const routes: RouteRecordRaw[] = [
  {
    path: '/reports',
    name: 'Reports',
    meta: { title: '报表中心', icon: 'bar-chart' },
    children: [
      {
        path: 'sales-stat',
        name: 'SalesStat',
        component: () => import('../../views/reports/SalesStat.vue'),
        meta: { title: '销售统计' }
      },
      {
        path: 'invoice-stat',
        name: 'InvoiceStat',
        component: () => import('../../views/reports/InvoiceStat.vue'),
        meta: { title: '发票统计' }
      },
      {
        path: 'po-query',
        name: 'POQuery',
        component: () => import('../../views/reports/POQuery.vue'),
        meta: { title: '采购订单查询' }
      },
      {
        path: 'inventory',
        name: 'Inventory',
        component: () => import('../../views/reports/Inventory.vue'),
        meta: { title: '库存报表' }
      },
      {
        path: 'snapshots',
        name: 'SnapshotHistory',
        component: () => import('../../views/reports/SnapshotHistory.vue'),
        meta: { title: '快照历史' }
      }
    ]
  }
]

export default routes

