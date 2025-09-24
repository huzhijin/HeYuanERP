namespace HeYuanERP.Domain.Entities;

// 领域实体：客户共享（AccountShare）
// 功能：将客户（Account）共享给指定用户，授予读/写等权限，可选过期时间；包含审计字段。
public class AccountShare
{
    // 主键
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    // 归属对象
    public string AccountId { get; set; } = string.Empty;   // 客户 Id
    public string TargetUserId { get; set; } = string.Empty; // 被共享用户 Id

    // 权限与有效期
    public string Permission { get; set; } = "read";        // 权限级别：read/edit（可扩展）
    public DateTime? ExpireAt { get; set; }                  // 过期时间（可选）

    // 审计字段
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 导航属性（可选）
    public Account? Account { get; set; }
    public User? TargetUser { get; set; }
}

