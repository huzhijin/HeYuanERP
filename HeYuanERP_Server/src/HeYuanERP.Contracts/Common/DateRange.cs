// 版权所有(c) HeYuanERP
// 说明：通用时间范围模型（中文注释）。

using System;

namespace HeYuanERP.Contracts.Common;

/// <summary>
/// 通用时间范围（UTC）。用于报表查询参数的时间过滤。
/// </summary>
public sealed class DateRange
{
    /// <summary>
    /// 起始时间（UTC，可空）。
    /// </summary>
    public DateTimeOffset? StartUtc { get; init; }

    /// <summary>
    /// 结束时间（UTC，可空）。
    /// </summary>
    public DateTimeOffset? EndUtc { get; init; }

    /// <summary>
    /// 是否为有效区间：当起止同时存在时，StartUtc ≤ EndUtc。
    /// </summary>
    public bool IsValid => !StartUtc.HasValue || !EndUtc.HasValue || StartUtc <= EndUtc;

    /// <summary>
    /// 规范化区间：
    /// - 若仅有开始时间，结束时间保持空；
    /// - 若仅有结束时间，开始时间保持空；
    /// - 若两者都有且顺序颠倒，则自动交换。
    /// </summary>
    public DateRange Normalize()
    {
        if (StartUtc.HasValue && EndUtc.HasValue && StartUtc > EndUtc)
        {
            return new DateRange { StartUtc = EndUtc, EndUtc = StartUtc };
        }
        return this;
    }

    public override string ToString()
        => $"{StartUtc:O} ~ {EndUtc:O}";
}

