namespace HeYuanERP.Api.Common;

// 权限码常量集中定义：资源.动作
// 说明：用于控制器声明与播种器一致，避免魔法字符串。
public static class PermissionCodes
{
    public static class Accounts
    {
        public const string Read = "accounts.read";
        public const string Create = "accounts.create";    // 同时用于编辑（暂）
        public const string Share = "accounts.share";
        public const string Transfer = "accounts.transfer";
    }

    public static class Orders
    {
        public const string Read = "orders.read";
        public const string Create = "orders.create";
        public const string Confirm = "orders.confirm";
        public const string Reverse = "orders.reverse";

        // 新增：订单状态管理权限
        public const string Submit = "orders.submit";           // 提交订单
        public const string Production = "orders.production";   // 生产管理
        public const string Delivery = "orders.delivery";       // 发货管理
        public const string Invoice = "orders.invoice";         // 开票管理
        public const string Close = "orders.close";             // 关闭订单
        public const string Cancel = "orders.cancel";           // 取消订单
    }

    public static class Deliveries
    {
        public const string Read = "deliveries.read";
        public const string Create = "deliveries.create";
        // 可选：细分送货打印权限；当前打印统一使用 print.read
        public const string Print = "deliveries.print";
    }

    public static class Returns
    {
        public const string Read = "returns.read";
        public const string Create = "returns.create";
    }

    public static class Purchase
    {
        public const string Read = "po.read";
        public const string Create = "po.create";
        // 扩展权限：导入/收货/打印（可选细分，默认可复用 create/print.read）
        public const string Import = "po.import";
        public const string Receive = "po.receive";
        public const string Print = "po.print";
    }

    public static class Inventory
    {
        public const string Read = "inventory.read";        // 汇总/事务查询
        // 其他入出库/调整（权限受限，预留）
        public const string In = "inventory.in";            // 其他入库
        public const string Out = "inventory.out";          // 其他出库
        public const string Adjust = "inventory.adjust";    // 盘点/调整
    }

    public static class Warehouses
    {
        public const string Read = "warehouses.read";
        public const string Create = "warehouses.create"; // 新增/编辑/删除
    }

    public static class Locations
    {
        public const string Read = "locations.read";
        public const string Create = "locations.create"; // 新增/编辑/删除
    }

    public static class Finance
    {
        public const string InvoicesRead = "invoices.read";
        public const string InvoicesCreate = "invoices.create";
        public const string InvoicesCancel = "invoices.cancel";
        public const string PaymentsRead = "payments.read";
    }

    public static class Invoice
    {
        public const string Read = "invoice.read";
        public const string Create = "invoice.create";
        public const string Cancel = "invoice.cancel";
        public const string Reconcile = "invoice.reconcile";    // 对账权限
        public const string Validate = "invoice.validate";      // 验证权限
    }

    public static class Reports
    {
        public const string Read = "reports.read";
    }

    public static class Print
    {
        public const string Read = "print.read";
    }
}
