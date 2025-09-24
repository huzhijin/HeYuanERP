using System.Globalization;
using HeYuanERP.Api.Data;
using HeYuanERP.Api.DTOs;
using HeYuanERP.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.Purchase;

// PO 导入服务实现：
// - 当前支持 CSV 文本作为占位（UTF-8，首行标题：productId,qty,unitPrice）；
// - 后续可替换为 NPOI/EPPlus 解析 .xlsx；
// - 预检：校验供应商存在、产品存在、数量单价有效，返回预览与错误列表；
// - 提交：在预检通过时创建一张采购单（单文件一单）。
public class POImportService : IPOImportService
{
    private readonly AppDbContext _db;

    public POImportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<POImportPrecheckResultDto> PrecheckAsync(string vendorId, IFormFile file, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(vendorId)) throw new ArgumentException("供应商必填", nameof(vendorId));
        if (!await _db.Accounts.AsNoTracking().AnyAsync(a => a.Id == vendorId, ct))
            throw new ApplicationException("供应商不存在");

        var rows = await ParseCsvAsync(file, ct);
        var result = new POImportPrecheckResultDto { VendorId = vendorId };
        result.TotalRecords = rows.Count;

        // 产品存在性
        var productIds = rows.Select(r => r.ProductId).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
        var existed = await _db.Products.AsNoTracking().Where(p => productIds.Contains(p.Id) && p.Active).Select(p => p.Id).ToListAsync(ct);
        var existSet = existed.ToHashSet();

        int rowNo = 1; // 从1开始计入表头
        foreach (var r in rows)
        {
            rowNo++;// 实际数据行号
            if (string.IsNullOrWhiteSpace(r.ProductId))
            {
                result.Errors.Add(new POImportErrorDto { RowNo = rowNo, Field = "productId", Message = "产品不能为空" });
                continue;
            }
            if (!existSet.Contains(r.ProductId))
            {
                result.Errors.Add(new POImportErrorDto { RowNo = rowNo, Field = "productId", Message = "产品不存在或已停用" });
                continue;
            }
            if (r.Qty <= 0)
            {
                result.Errors.Add(new POImportErrorDto { RowNo = rowNo, Field = "qty", Message = "数量需大于0" });
                continue;
            }
            if (r.UnitPrice < 0)
            {
                result.Errors.Add(new POImportErrorDto { RowNo = rowNo, Field = "unitPrice", Message = "单价不能为负" });
                continue;
            }

            if (result.Preview.Count < 10)
            {
                result.Preview.Add(new POImportPreviewItemDto
                {
                    ProductId = r.ProductId,
                    Qty = r.Qty,
                    UnitPrice = r.UnitPrice
                });
            }
        }

        result.InvalidRecords = result.Errors.Select(e => e.RowNo).Distinct().Count();
        result.ValidRecords = result.TotalRecords - result.InvalidRecords;
        return result;
    }

    public async Task<POImportReceiptDto> ImportAsync(string vendorId, IFormFile file, string currentUserId, CancellationToken ct)
    {
        var pre = await PrecheckAsync(vendorId, file, ct);
        if (pre.InvalidRecords > 0)
        {
            throw new ApplicationException($"预检失败：{pre.InvalidRecords} 条无效记录");
        }

        var rows = await ParseCsvAsync(file, ct);
        using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var po = new PurchaseOrder
            {
                PoNo = await NextPoNoAsync(ct),
                VendorId = vendorId,
                PoDate = DateTime.UtcNow.Date,
                Status = "draft",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId
            };
            foreach (var r in rows)
            {
                po.Lines.Add(new POLine
                {
                    ProductId = r.ProductId,
                    Qty = r.Qty,
                    UnitPrice = r.UnitPrice,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                });
            }

            _db.PurchaseOrders.Add(po);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return new POImportReceiptDto
            {
                VendorId = vendorId,
                PoId = po.Id,
                PoNo = po.PoNo,
                CreatedLines = po.Lines.Count,
                SkippedLines = 0,
                Message = "导入成功"
            };
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<List<Row>> ParseCsvAsync(IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0) throw new ApplicationException("上传文件为空");
        // 检查扩展名/类型（占位，不强制）
        // 读取为文本（UTF-8），按\n 分行，首行标题，逗号分隔
        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2) return new List<Row>();

        // 标题校验（需要包含 productId, qty, unitPrice，大小写不敏感）
        var header = lines[0].Split(',').Select(s => s.Trim()).ToList();
        int idxProduct = IndexOf(header, "productId");
        int idxQty = IndexOf(header, "qty");
        int idxPrice = IndexOf(header, "unitPrice");
        if (idxProduct < 0 || idxQty < 0 || idxPrice < 0)
            throw new ApplicationException("文件格式错误：缺少标题列 productId/qty/unitPrice（CSV 占位格式）");

        var rows = new List<Row>();
        for (int i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split(',');
            string productId = SafeGet(cols, idxProduct);
            decimal qty = ParseDecimal(SafeGet(cols, idxQty));
            decimal price = ParseDecimal(SafeGet(cols, idxPrice));
            rows.Add(new Row { ProductId = productId.Trim(), Qty = qty, UnitPrice = price });
        }
        return rows;
    }

    private static int IndexOf(List<string> header, string name)
    {
        for (int i = 0; i < header.Count; i++)
        {
            if (string.Equals(header[i], name, StringComparison.OrdinalIgnoreCase)) return i;
        }
        return -1;
    }

    private static string SafeGet(string[] cols, int index)
        => index >= 0 && index < cols.Length ? cols[index] : string.Empty;

    private static decimal ParseDecimal(string s)
    {
        if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v)) return v;
        if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v)) return v;
        return 0m;
    }

    private async Task<string> NextPoNoAsync(CancellationToken ct)
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        for (int i = 0; i < 5; i++)
        {
            var rnd = Random.Shared.Next(0, 9999).ToString("D4");
            var no = $"PO{date}-{rnd}";
            var exists = await _db.PurchaseOrders.AsNoTracking().AnyAsync(o => o.PoNo == no, ct);
            if (!exists) return no;
        }
        return $"PO{date}-{DateTime.UtcNow:HHmmssfff}";
    }

    private class Row
    {
        public string ProductId { get; set; } = string.Empty;
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

