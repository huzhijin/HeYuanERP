// 版权所有(c) HeYuanERP
// 说明：报表参数白名单与绑定实现（中文注释）。

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using HeYuanERP.Application.Common.Validation;
using HeYuanERP.Contracts.Common;
using HeYuanERP.Contracts.Reports;
using HeYuanERP.Domain.Reports;

namespace HeYuanERP.Application.Reports.Validation;

/// <summary>
/// 报表参数白名单接口。
/// </summary>
public interface IReportParameterWhitelist
{
    /// <summary>
    /// 过滤非白名单参数，并进行必要的规范化（如 from/to 转换为 range.startUtc/endUtc）。
    /// 返回用于记录与导出的安全参数字典（键名使用 camelCase）。
    /// </summary>
    IDictionary<string, object?> Filter(ReportType type, IDictionary<string, object?> input);

    /// <summary>
    /// 将安全参数绑定为强类型对象（基于 System.Text.Json 匹配）。
    /// </summary>
    T Bind<T>(ReportType type, IDictionary<string, object?> input) where T : class, new();
}

/// <summary>
/// 报表参数白名单实现。
/// </summary>
public class ReportParameterWhitelist : IReportParameterWhitelist
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public IDictionary<string, object?> Filter(ReportType type, IDictionary<string, object?> input)
    {
        input ??= new Dictionary<string, object?>();

        // 解包 params 层（若存在）
        if (input.TryGetValue("params", out var p) && p is IDictionary<string, object?> paramsDict)
        {
            input = paramsDict;
        }

        // 统一键名为小写以便匹配
        var flat = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in input)
        {
            flat[kv.Key] = kv.Value;
        }

        // 通用：时间范围别名处理（from/to/start/end -> range.startUtc/endUtc）
        var range = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var from = flat.GetAs<string>("from") ?? flat.GetAs<string>("start") ?? flat.GetAs<string>("startUtc");
        var to = flat.GetAs<string>("to") ?? flat.GetAs<string>("end") ?? flat.GetAs<string>("endUtc");
        if (!string.IsNullOrWhiteSpace(from)) range["startUtc"] = ParseIso(from);
        if (!string.IsNullOrWhiteSpace(to)) range["endUtc"] = ParseIso(to);
        // 也支持传入对象 range
        if (flat.TryGetValue("range", out var rv) && rv is IDictionary<string, object?> rdict)
        {
            var rstart = rdict.GetAs<string>("startUtc") ?? rdict.GetAs<string>("start") ?? rdict.GetAs<string>("from");
            var rend = rdict.GetAs<string>("endUtc") ?? rdict.GetAs<string>("end") ?? rdict.GetAs<string>("to");
            if (!string.IsNullOrWhiteSpace(rstart)) range["startUtc"] = ParseIso(rstart);
            if (!string.IsNullOrWhiteSpace(rend)) range["endUtc"] = ParseIso(rend);
        }

        // 针对类型的白名单集合
        var allowed = AllowedKeys(type);
        var safe = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        void Put(string key, object? val)
        {
            if (val is null) return;
            if (!allowed.Contains(key)) return;
            safe[key] = val;
        }

        // 通用分页（若在 allowed 中）
        int? page = flat.GetAs<int?>("page");
        int? size = flat.GetAs<int?>("size");
        if (page.HasValue) Put("page", Math.Max(1, page.Value));
        if (size.HasValue) Put("size", Math.Clamp(size.Value, 1, 200));

        // 类型特定字段
        switch (type)
        {
            case ReportType.SalesStat:
                Put("range", range.Count > 0 ? range : null);
                Put("customerId", flat.GetAs<string>("customerId"));
                Put("salesmanId", flat.GetAs<string>("salesmanId"));
                Put("productId", flat.GetAs<string>("productId"));
                Put("currency", flat.GetAs<string>("currency"));
                Put("groupBy", NormalizeGroup(flat.GetAs<string>("groupBy"), new[] { "day", "month", "product", "salesman", "customer" }));
                break;

            case ReportType.InvoiceStat:
                Put("range", range.Count > 0 ? range : null);
                Put("accountId", flat.GetAs<string>("accountId"));
                Put("status", flat.GetAs<string>("status"));
                Put("currency", flat.GetAs<string>("currency"));
                Put("groupBy", NormalizeGroup(flat.GetAs<string>("groupBy"), new[] { "day", "month", "account" }));
                break;

            case ReportType.POQuery:
                Put("range", range.Count > 0 ? range : null);
                Put("vendorId", flat.GetAs<string>("vendorId"));
                Put("status", flat.GetAs<string>("status"));
                break;

            case ReportType.Inventory:
                Put("productId", flat.GetAs<string>("productId"));
                Put("whse", flat.GetAs<string>("whse"));
                Put("loc", flat.GetAs<string>("loc"));
                Put("range", range.Count > 0 ? range : null);
                break;
        }

        return safe;
    }

    public T Bind<T>(ReportType type, IDictionary<string, object?> input) where T : class, new()
    {
        var safe = Filter(type, input);
        var json = JsonSerializer.Serialize(safe, JsonOptions);
        var result = JsonSerializer.Deserialize<T>(json, JsonOptions) ?? new T();
        // 提示：Range 为 init-only 属性，规范化逻辑已在 Filter 阶段完成键映射，
        // 这里不再对 Range 进行二次赋值以避免违反 init-only 约束。
        return result;
    }

    private static ISet<string> AllowedKeys(ReportType type)
    {
        return type switch
        {
            ReportType.SalesStat => new HashSet<string>(new[] { "range", "customerId", "salesmanId", "productId", "currency", "groupBy" }, StringComparer.OrdinalIgnoreCase),
            ReportType.InvoiceStat => new HashSet<string>(new[] { "range", "accountId", "status", "currency", "groupBy" }, StringComparer.OrdinalIgnoreCase),
            ReportType.POQuery => new HashSet<string>(new[] { "range", "vendorId", "status", "page", "size" }, StringComparer.OrdinalIgnoreCase),
            ReportType.Inventory => new HashSet<string>(new[] { "productId", "whse", "loc", "range", "page", "size" }, StringComparer.OrdinalIgnoreCase),
            _ => new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        };
    }

    private static string? NormalizeGroup(string? group, IEnumerable<string> allowed)
    {
        if (string.IsNullOrWhiteSpace(group)) return null;
        var g = group.Trim().ToLowerInvariant();
        return allowed.Contains(g) ? g : null;
    }

    private static string ParseIso(string s)
    {
        // 对日期/日期时间统一转为 ISO8601 字符串，便于 JSON 绑定为 DateTimeOffset?
        if (DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dto))
        {
            return dto.ToUniversalTime().ToString("O");
        }
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt))
        {
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc).ToString("O");
        }
        // 尝试仅日期（当作本地日期 00:00:00Z）
        if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
        {
            var dt2 = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, DateTimeKind.Utc);
            return dt2.ToString("O");
        }
        return s; // 保留原值
    }
}
