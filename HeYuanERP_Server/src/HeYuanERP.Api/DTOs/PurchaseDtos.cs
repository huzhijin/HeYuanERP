namespace HeYuanERP.Api.DTOs;

// 采购（PO）相关 DTO：用于列表/详情/新建/编辑/确认/收货

// 列表查询
public class POListQueryDto
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
    public string? Keyword { get; set; }        // 采购单号模糊
    public string? VendorId { get; set; }       // 供应商（AccountId）
    public DateTime? From { get; set; }         // 起始日期（PoDate）
    public DateTime? To { get; set; }           // 截止日期（PoDate）
    public string? Status { get; set; }         // 状态：draft/confirmed
}

// 列表项
public class POListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string PoNo { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public DateTime PoDate { get; set; }
    public string Status { get; set; } = "draft";
    public string? Remark { get; set; }

    // 汇总
    public decimal TotalQty { get; set; }
    public decimal TotalAmount { get; set; }
}

// 详情（含行）
public class PODetailDto
{
    public string Id { get; set; } = string.Empty;
    public string PoNo { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public DateTime PoDate { get; set; }
    public string Status { get; set; } = "draft";
    public string? Remark { get; set; }

    public List<POLineDto> Lines { get; set; } = new();

    // 审计
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 汇总
    public decimal TotalQty { get; set; }
    public decimal TotalAmount { get; set; }
}

public class POLineDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
}

// 新建/编辑输入
public class POCreateDto
{
    public string VendorId { get; set; } = string.Empty;
    public DateTime PoDate { get; set; } = DateTime.UtcNow.Date;
    public string? Remark { get; set; }
    public List<POLineCreateDto> Lines { get; set; } = new();
}

public class POLineCreateDto
{
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
}

public class POUpdateDto
{
    public string VendorId { get; set; } = string.Empty;
    public DateTime PoDate { get; set; } = DateTime.UtcNow.Date;
    public string? Remark { get; set; }
    public List<POLineUpdateDto> Lines { get; set; } = new();
}

public class POLineUpdateDto : POLineCreateDto
{
    public string? Id { get; set; }          // 行Id（可为空：新增行）
    public bool? _deleted { get; set; }      // 标记删除
}

// 确认输入（预留）
public class POConfirmDto
{
    public string Id { get; set; } = string.Empty;
}

// 收货输入
public class POReceiveCreateDto
{
    public DateTime ReceiveDate { get; set; } = DateTime.UtcNow.Date;
    public string? Remark { get; set; }
    public List<POReceiveLineCreateDto> Lines { get; set; } = new();
}

public class POReceiveLineCreateDto
{
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string Whse { get; set; } = string.Empty;
    public string? Loc { get; set; }
}

// 收货结果
public class POReceiveDetailDto
{
    public string Id { get; set; } = string.Empty;   // 收货单 Id
    public string PoId { get; set; } = string.Empty;
    public DateTime ReceiveDate { get; set; }
    public string Status { get; set; } = "completed";
    public string? Remark { get; set; }
    public List<POReceiveLineDto> Lines { get; set; } = new();
}

public class POReceiveLineDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string? Whse { get; set; }
    public string? Loc { get; set; }
}

