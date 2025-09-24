namespace HeYuanERP.Api.DTOs;

// PO Excel 导入：预检与回执 DTO

// 预检结果
public class POImportPrecheckResultDto
{
    public string VendorId { get; set; } = string.Empty;  // 供应商 Id
    public int TotalRecords { get; set; }
    public int ValidRecords { get; set; }
    public int InvalidRecords { get; set; }
    public List<POImportErrorDto> Errors { get; set; } = new();
    public List<POImportPreviewItemDto> Preview { get; set; } = new(); // 最多10条预览
}

// 回执（导入提交结果）
public class POImportReceiptDto
{
    public string VendorId { get; set; } = string.Empty;
    public string PoId { get; set; } = string.Empty;
    public string PoNo { get; set; } = string.Empty;
    public int CreatedLines { get; set; }
    public int SkippedLines { get; set; }
    public string Message { get; set; } = "导入成功";
}

// 错误项（逐行）
public class POImportErrorDto
{
    public int RowNo { get; set; }                 // 源文件行号（含标题行从1开始）
    public string? Field { get; set; }             // 字段（如 productId/qty/unitPrice）
    public string Message { get; set; } = string.Empty; // 错误说明
}

// 预览项（规范化）
public class POImportPreviewItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
}

