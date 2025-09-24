// 版权所有(c) HeYuanERP
// 说明：应用层通用校验与数据转换扩展（中文注释）。

using System;
using System.Collections.Generic;
using System.Globalization;

namespace HeYuanERP.Application.Common.Validation;

/// <summary>
/// 通用校验与转换扩展。
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// 尝试将对象转换为 <typeparamref name="T"/>。支持基础类型与可空类型。
    /// 转换失败时返回 default。
    /// </summary>
    public static T? As<T>(this object? value)
    {
        if (value is null) return default;
        var t = typeof(T);
        var underlying = Nullable.GetUnderlyingType(t) ?? t;

        try
        {
            if (underlying.IsEnum)
            {
                if (value is string s1 && Enum.TryParse(underlying, s1, true, out var e))
                    return (T?)e;
                if (value is IConvertible c1)
                    return (T?)Enum.ToObject(underlying, c1.ToInt32(CultureInfo.InvariantCulture));
                return default;
            }

            if (underlying == typeof(Guid))
            {
                if (value is Guid g) return (T)(object)g;
                if (Guid.TryParse(value.ToString(), out var g2)) return (T)(object)g2;
                return default;
            }

            if (underlying == typeof(DateTimeOffset))
            {
                if (value is DateTimeOffset dto) return (T)(object)dto;
                if (DateTimeOffset.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dto2)) return (T)(object)dto2;
                return default;
            }

            if (underlying == typeof(DateTime))
            {
                if (value is DateTime dt) return (T)(object)dt;
                if (DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt2)) return (T)(object)dt2;
                return default;
            }

            if (underlying == typeof(DateOnly))
            {
                if (value is DateOnly d1) return (T)(object)d1;
                if (DateOnly.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var d2)) return (T)(object)d2;
                return default;
            }

            if (underlying == typeof(TimeOnly))
            {
                if (value is TimeOnly t1) return (T)(object)t1;
                if (TimeOnly.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var t2)) return (T)(object)t2;
                return default;
            }

            if (value is IConvertible conv)
            {
                return (T)Convert.ChangeType(conv, underlying, CultureInfo.InvariantCulture);
            }

            // 其他引用类型直接尝试强转
            if (value is T tval) return tval;
            return default;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 从字典中获取 key 并转换为 T，失败返回 default。
    /// </summary>
    public static T? GetAs<T>(this IDictionary<string, object?> dict, string key)
    {
        if (dict.TryGetValue(key, out var v)) return v.As<T>();
        return default;
    }
}

