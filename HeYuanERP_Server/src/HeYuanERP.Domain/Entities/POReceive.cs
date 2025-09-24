namespace HeYuanERP.Domain.Entities;

// 领域实体：采购收货（头）
// OpenAPI 对齐：/api/po/{id}/receive（接收入库动作），此处建模为独立收货单
public class POReceive
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 归属
    public string PoId { get; set; } = string.Empty;       // 关联采购单 Id

    // 基本
    public DateTime ReceiveDate { get; set; } = DateTime.UtcNow.Date;
    public string Status { get; set; } = "draft";          // 草稿/完成 等
    public string? Remark { get; set; }

    // 审计
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航
    public PurchaseOrder? Po { get; set; }
    public ICollection<POReceiveLine> Lines { get; set; } = new List<POReceiveLine>();
}

