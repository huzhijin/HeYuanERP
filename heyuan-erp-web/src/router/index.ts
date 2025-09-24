// 路由配置：含基础布局与路由守卫（标题/鉴权占位）
import { createRouter, createWebHistory, type NavigationGuardNext, type RouteLocationNormalized } from 'vue-router';
import BasicLayout from '@/layouts/BasicLayout.vue';
import Dashboard from '@/views/Dashboard.vue';
import { useAuthStore } from '@/store/auth';

// 路由结构：
// - 根路由使用基础布局，并在其下配置实际业务页面（子路由）
// - 登录单独路由，便于未登录跳转
const routes = [
  {
    path: '/',
    name: 'Root',
    component: BasicLayout,
    meta: { title: '控制台', requiresAuth: false },
    children: [
      { path: '', name: 'Dashboard', component: Dashboard, meta: { title: '工作台', requiresAuth: true } },
      // 客户
      { path: 'accounts', name: 'AccountList', component: () => import('@/views/accounts/AccountList.vue'), meta: { title: '客户', requiresAuth: true } },
      { path: 'accounts/new', name: 'AccountCreate', component: () => import('@/views/accounts/AccountForm.vue'), meta: { title: '新建客户', requiresAuth: true } },
      { path: 'accounts/:id', name: 'AccountEdit', component: () => import('@/views/accounts/AccountForm.vue'), meta: { title: '编辑客户', requiresAuth: true } },
      // 订单
      { path: 'orders', name: 'OrderList', component: () => import('@/views/orders/OrderList.vue'), meta: { title: '订单', requiresAuth: true } },
      { path: 'orders/new', name: 'OrderCreate', component: () => import('@/views/orders/OrderForm.vue'), meta: { title: '新建订单', requiresAuth: true } },
      { path: 'orders/:id', name: 'OrderEdit', component: () => import('@/views/orders/OrderForm.vue'), meta: { title: '编辑订单', requiresAuth: true } },
      { path: 'orders/:id/print', name: 'OrderPrintPreview', component: () => import('@/views/orders/PrintPreview.vue'), meta: { title: '打印预览', requiresAuth: true } },
      // 采购
      { path: 'purchase/po', name: 'POList', component: () => import('@/views/purchase/POList.vue'), meta: { title: '采购单', requiresAuth: true } },
      { path: 'purchase/po/new', name: 'POCreate', component: () => import('@/views/purchase/POForm.vue'), meta: { title: '新建采购单', requiresAuth: true } },
      { path: 'purchase/po/:id', name: 'POEdit', component: () => import('@/views/purchase/POForm.vue'), meta: { title: '编辑采购单', requiresAuth: true } },
      { path: 'purchase/po/:id/receive', name: 'POReceive', component: () => import('@/views/purchase/POReceive.vue'), meta: { title: '采购收货', requiresAuth: true } },
      { path: 'purchase/po/import', name: 'POImport', component: () => import('@/views/purchase/POImport.vue'), meta: { title: '采购导入', requiresAuth: true } },
      // 库存
      { path: 'inventory/summary', name: 'InventorySummary', component: () => import('@/views/inventory/Summary.vue'), meta: { title: '库存汇总', requiresAuth: true } },
      { path: 'inventory/transactions', name: 'InventoryTransactions', component: () => import('@/views/inventory/Transactions.vue'), meta: { title: '库存事务', requiresAuth: true } },
      { path: 'inventory/alerts', name: 'InventoryAlerts', component: () => import('@/views/inventory/Alerts.vue'), meta: { title: '库存预警', requiresAuth: true } },
      { path: 'inventory/warehouses', name: 'Warehouses', component: () => import('@/views/inventory/Warehouses.vue'), meta: { title: '仓库', requiresAuth: true } },
      { path: 'inventory/locations', name: 'Locations', component: () => import('@/views/inventory/Locations.vue'), meta: { title: '库位', requiresAuth: true } },
      // 发票
      { path: 'invoices', name: 'InvoiceList', component: () => import('@/views/invoices/InvoiceList.vue'), meta: { title: '发票', requiresAuth: true } },
      { path: 'invoices/new', name: 'InvoiceCreate', component: () => import('@/views/invoices/InvoiceForm.vue'), meta: { title: '新建发票', requiresAuth: true } },
      { path: 'invoices/:id', name: 'InvoiceDetail', component: () => import('@/views/invoices/InvoiceDetail.vue'), meta: { title: '发票详情', requiresAuth: true } },
      // CRM
      { path: 'crm/opportunities', name: 'OpportunityList', component: () => import('@/views/crm/OpportunityList.vue'), meta: { title: '销售机会', requiresAuth: true } },
      { path: 'crm/visits', name: 'VisitList', component: () => import('@/views/crm/VisitList.vue'), meta: { title: '客户拜访', requiresAuth: true } },
      // 产品与价格
      { path: 'product/categories', name: 'CategoryList', component: () => import('@/views/product/CategoryList.vue'), meta: { title: '产品分类', requiresAuth: true } },
      { path: 'product/products', name: 'ProductList', component: () => import('@/views/product/ProductList.vue'), meta: { title: '产品', requiresAuth: true } },
      { path: 'product/strategies', name: 'PriceStrategyList', component: () => import('@/views/product/PriceStrategyList.vue'), meta: { title: '价格策略', requiresAuth: true } },
      { path: 'product/quotations', name: 'QuotationList', component: () => import('@/views/product/QuotationList.vue'), meta: { title: '报价单', requiresAuth: true } },
      // 财务 AR/AP
      { path: 'finance/receivables', name: 'ReceivableList', component: () => import('@/views/finance/ReceivableList.vue'), meta: { title: '应收', requiresAuth: true } },
      { path: 'finance/payables', name: 'PayableList', component: () => import('@/views/finance/PayableList.vue'), meta: { title: '应付', requiresAuth: true } },
      { path: 'finance/dashboard', name: 'FinanceDashboard', component: () => import('@/views/finance/FinanceDashboard.vue'), meta: { title: '财务仪表板', requiresAuth: true } },
      // 收款 / 对账
      { path: 'payments', name: 'PaymentsList', component: () => import('@/pages/payments/PaymentList.vue'), meta: { title: '收款', requiresAuth: true } },
      { path: 'payments/create', name: 'PaymentCreate', component: () => import('@/pages/payments/PaymentCreate.vue'), meta: { title: '新增收款', requiresAuth: true } },
      { path: 'reconciliation/export', name: 'ReconciliationExport', component: () => import('@/pages/reconciliation/ReconciliationExport.vue'), meta: { title: '对账导出', requiresAuth: true } },
      // 物流
      { path: 'deliveries', name: 'DeliveryList', component: () => import('@/views/logistics/DeliveryList.vue'), meta: { title: '送货单', requiresAuth: true } },
      { path: 'deliveries/new', name: 'DeliveryCreate', component: () => import('@/views/logistics/DeliveryForm.vue'), meta: { title: '新建送货单', requiresAuth: true } },
      { path: 'deliveries/:id', name: 'DeliveryEdit', component: () => import('@/views/logistics/DeliveryForm.vue'), meta: { title: '编辑送货单', requiresAuth: true } },
      { path: 'returns', name: 'ReturnList', component: () => import('@/views/logistics/ReturnList.vue'), meta: { title: '退货单', requiresAuth: true } },
      { path: 'returns/new', name: 'ReturnCreate', component: () => import('@/views/logistics/ReturnForm.vue'), meta: { title: '新建退货单', requiresAuth: true } },
      { path: 'returns/:id', name: 'ReturnEdit', component: () => import('@/views/logistics/ReturnForm.vue'), meta: { title: '编辑退货单', requiresAuth: true } },
      // 报表中心与报表
      { path: 'reports', name: 'ReportCenter', component: () => import('@/views/reports/ReportCenter.vue'), meta: { title: '报表中心', requiresAuth: true } },
      { path: 'reports/advanced', name: 'AdvancedReports', component: () => import('@/views/reports/Advanced.vue'), meta: { title: '高级报表', requiresAuth: true } },
      { path: 'reports/sales-stat', name: 'SalesStat', component: () => import('@/views/reports/SalesStat.vue'), meta: { title: '销售统计', requiresAuth: true } },
      { path: 'reports/invoice-stat', name: 'InvoiceStat', component: () => import('@/views/reports/InvoiceStat.vue'), meta: { title: '发票统计', requiresAuth: true } },
      { path: 'reports/po-query', name: 'POQuery', component: () => import('@/views/reports/POQuery.vue'), meta: { title: '采购订单查询', requiresAuth: true } },
      { path: 'reports/inventory', name: 'InventoryReport', component: () => import('@/views/reports/Inventory.vue'), meta: { title: '库存报表', requiresAuth: true } },
      { path: 'reports/snapshots', name: 'SnapshotHistory', component: () => import('@/views/reports/SnapshotHistory.vue'), meta: { title: '快照历史', requiresAuth: true } },
      // 对账
      { path: 'reconciliation/differences', name: 'ReconciliationDifferences', component: () => import('@/views/reconciliation/DifferenceList.vue'), meta: { title: '对账差异', requiresAuth: true } },
    ],
  },
  // 登录页
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/Login.vue'),
    meta: { title: '登录', requiresAuth: false },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// 路由守卫：
// - 设置页面标题：路由 meta.title + 应用名
// - 鉴权占位：当 meta.requiresAuth=true 且未登录时，跳转登录页
router.beforeEach((to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext) => {
  const appName = import.meta.env.VITE_APP_NAME || 'HeYuanERP';
  const title = (to.meta?.title as string | undefined) ?? '';
  document.title = title ? `${title} - ${appName}` : appName;

  const auth = useAuthStore();
  if (to.meta?.requiresAuth && !auth.isAuthenticated) {
    next({ name: 'Login', query: { redirect: to.fullPath } });
    return;
  }
  next();
});

export default router;
