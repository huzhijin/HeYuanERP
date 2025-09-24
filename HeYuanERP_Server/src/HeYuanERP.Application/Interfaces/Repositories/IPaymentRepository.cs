using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Domain.Entities;

namespace HeYuanERP.Application.Interfaces.Repositories;

/// <summary>
/// 收款仓储接口（应用层抽象，由基础设施实现）。
/// 说明：返回领域实体，由上层应用服务负责映射为 DTO 及统一响应。
/// </summary>
public interface IPaymentRepository
{
    /// <summary>
    /// 新增收款（包含附件）。
    /// </summary>
    /// <param name="payment">包含附件的收款实体</param>
    /// <param name="ct">取消令牌</param>
    Task<Payment> AddAsync(Payment payment, CancellationToken ct = default);

    /// <summary>
    /// 根据 Id 获取收款（包含附件）。
    /// </summary>
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// 分页查询收款列表（支持条件筛选与排序）。
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <param name="page">页码（1 开始）</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="sortBy">排序字段：paymentDate/amount/createdAt（默认 paymentDate）</param>
    /// <param name="sortDesc">是否倒序（默认 true）</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>元组：(Items, TotalCount)</returns>
    Task<(IReadOnlyList<Payment> Items, long TotalCount)> QueryAsync(
        PaymentQueryParameters query,
        int page,
        int pageSize,
        string? sortBy,
        bool sortDesc,
        CancellationToken ct = default);
}

/// <summary>
/// 收款查询参数。
/// </summary>
public sealed class PaymentQueryParameters
{
    /// <summary>
    /// 收款方式（可选）。
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// 金额下限（可选）。
    /// </summary>
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// 金额上限（可选）。
    /// </summary>
    public decimal? MaxAmount { get; set; }

    /// <summary>
    /// 起始日期（含）。
    /// </summary>
    public DateOnly? DateFrom { get; set; }

    /// <summary>
    /// 截止日期（含）。
    /// </summary>
    public DateOnly? DateTo { get; set; }

    /// <summary>
    /// 关键字（备注模糊匹配）。
    /// </summary>
    public string? Keyword { get; set; }
}

