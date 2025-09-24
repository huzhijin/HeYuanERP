using System;

namespace HeYuanERP.Application.AI
{
    /// <summary>
    /// 定价请求。
    /// </summary>
    public class PriceRequest
    {
        /// <summary>
        /// 商品编码。
        /// </summary>
        public string Sku { get; set; } = string.Empty;

        /// <summary>
        /// 成本（含税）。
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 期望毛利率（0-1，可选）。
        /// </summary>
        public decimal? TargetMargin { get; set; }

        /// <summary>
        /// 货币（默认 CNY）。
        /// </summary>
        public string Currency { get; set; } = "CNY";
    }

    /// <summary>
    /// 定价响应。
    /// </summary>
    public class PriceResponse
    {
        public string Sku { get; set; } = string.Empty;
        public decimal RecommendedPrice { get; set; }
        public string Currency { get; set; } = "CNY";
        public string Rationale { get; set; } = string.Empty;
        public DateTimeOffset GeneratedAt { get; set; }
    }

    /// <summary>
    /// 预测请求。
    /// </summary>
    public class ForecastRequest
    {
        public string Sku { get; set; } = string.Empty;
        /// <summary>
        /// 预测周期（周）。
        /// </summary>
        public int HorizonWeeks { get; set; } = 4;
    }

    /// <summary>
    /// 预测响应。
    /// </summary>
    public class ForecastResponse
    {
        public string Sku { get; set; } = string.Empty;
        /// <summary>
        /// 预计销量（周期总计）。
        /// </summary>
        public int ExpectedUnits { get; set; }
        public DateTimeOffset GeneratedAt { get; set; }
    }

    /// <summary>
    /// 信用评估请求。
    /// </summary>
    public class CreditRequest
    {
        public string CustomerId { get; set; } = string.Empty;
        /// <summary>
        /// 本次申请额度。
        /// </summary>
        public decimal RequestedAmount { get; set; }
    }

    /// <summary>
    /// 信用评估响应。
    /// </summary>
    public class CreditResponse
    {
        public string CustomerId { get; set; } = string.Empty;
        /// <summary>
        /// 信用评分（0-100）。
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 建议授信额度。
        /// </summary>
        public decimal SuggestedLimit { get; set; }
        public string Advice { get; set; } = string.Empty;
        public DateTimeOffset GeneratedAt { get; set; }
    }
}

