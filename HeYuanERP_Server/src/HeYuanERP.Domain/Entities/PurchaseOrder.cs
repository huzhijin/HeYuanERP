namespace HeYuanERP.Domain.Entities;

// 领域实体：采购订单（头）
// OpenAPI 对齐：id, poNo, vendorId, poDate, status, lines
public class PurchaseOrder
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 基本信息
    public string PoNo { get; set; } = string.Empty;      // 采购单号（业务唯一）
    public string VendorId { get; set; } = string.Empty;  // 供应商 Id（此处沿用 Account 作为供应商）
    public DateTime PoDate { get; set; } = DateTime.UtcNow.Date;
    public string Status { get; set; } = "draft";
    public string? Remark { get; set; }

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public Account? Vendor { get; set; }
    public ICollection<POLine> Lines { get; set; } = new List<POLine>();
}

