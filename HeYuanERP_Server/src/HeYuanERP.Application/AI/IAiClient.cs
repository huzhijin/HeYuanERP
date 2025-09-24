using System.Threading;
using System.Threading.Tasks;

namespace HeYuanERP.Application.AI
{
    /// <summary>
    /// AI 服务客户端接口（定价/预测/信用）。
    /// 初期使用 Mock 实现，通过 REST 接口契约对齐 OpenAPI。
    /// </summary>
    public interface IAiClient
    {
        /// <summary>
        /// 获取智能定价建议。
        /// </summary>
        /// <param name="request">定价请求</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<PriceResponse> GetPriceAsync(PriceRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// 销售预测。
        /// </summary>
        /// <param name="request">预测请求</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<ForecastResponse> ForecastAsync(ForecastRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// 客户信用评估。
        /// </summary>
        /// <param name="request">信用评估请求</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<CreditResponse> CreditAssessAsync(CreditRequest request, CancellationToken cancellationToken = default);
    }

    // 说明：DTO 类型将在后续批次提供（AiDtos.cs）。
}

