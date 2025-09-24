using HeYuanERP.Api.Common;

namespace HeYuanERP.Api.DTOs;

// 账户（客户）相关 DTO 定义：列表/表单/共享/转移/拜访/附件

// 列表查询参数
public class AccountListQueryDto
{
    public int Page { get; set; } = 1;         // 页码（从 1 开始）
    public int Size { get; set; } = 20;        // 每页大小（建议 10~100）
    public string? Keyword { get; set; }       // 关键字（匹配 Code/Name）
    public bool? Active { get; set; }          // 过滤启用状态
    public string? OwnerId { get; set; }       // 过滤负责人
}

// 列表项
public class AccountListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? OwnerId { get; set; }
    public bool Active { get; set; }
    public DateTime? LastEventDate { get; set; }
    public string? Description { get; set; }
}

// 详情
public class AccountDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? OwnerId { get; set; }
    public string? TaxNo { get; set; }
    public bool Active { get; set; }
    public DateTime? LastEventDate { get; set; }
    public string? Description { get; set; }

    // 审计
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

// 新建/编辑
public class AccountCreateDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? OwnerId { get; set; }
    public string? TaxNo { get; set; }
    public bool Active { get; set; } = true;
    public string? Description { get; set; }
}

public class AccountUpdateDto
{
    public string? Code { get; set; }           // 若提供则支持修改编码（将执行唯一校验）
    public string Name { get; set; } = string.Empty;
    public string? OwnerId { get; set; }
    public string? TaxNo { get; set; }
    public bool Active { get; set; } = true;
    public string? Description { get; set; }
}

// 共享/转移
public class AccountShareRequestDto
{
    public string TargetUserId { get; set; } = string.Empty;
    public string Permission { get; set; } = "read";  // read/edit
    public DateTime? ExpireAt { get; set; }
}

public class AccountTransferRequestDto
{
    public string NewOwnerId { get; set; } = string.Empty;
}

// 拜访
public class AccountVisitCreateDto
{
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;
    public string? ContactId { get; set; }
    public string? VisitorId { get; set; } // 若不传则后端使用当前用户
    public string? Subject { get; set; }
    public string? Content { get; set; }
    public string? Location { get; set; }
    public string? Result { get; set; }
    public DateTime? NextActionAt { get; set; }
}

public class AccountVisitDto
{
    public string Id { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string? ContactId { get; set; }
    public string? VisitorId { get; set; }
    public DateTime VisitDate { get; set; }
    public string? Subject { get; set; }
    public string? Content { get; set; }
    public string? Location { get; set; }
    public string? Result { get; set; }
    public DateTime? NextActionAt { get; set; }

    // 审计
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

// 附件（仅列表展示，本批不含上传）
public class AttachmentDto
{
    public string Id { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long Size { get; set; }
    public string StorageUri { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string? UploadedBy { get; set; }
}

