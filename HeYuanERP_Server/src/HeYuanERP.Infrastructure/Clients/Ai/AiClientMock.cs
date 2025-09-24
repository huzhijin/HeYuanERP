using System;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.AI;
using HeYuanERP.Infrastructure.Logging.Audit;

namespace HeYuanERP.Infrastructure.Clients.Ai
{
    /// <summary>
    /// AI 客户端 Mock 实现（定价/预测/信用）。
    /// 特性：纯内存计算，无外部调用，快速稳定，不影响主链路。
    /// </summary>
    public class AiClientMock : IAiClient
    {
        private readonly IAuditLogger _audit;

        public AiClientMock(IAuditLogger auditLogger)
        {
            _audit = auditLogger;
            _audit.LogReplacementNotice(
                component: nameof(IAiClient),
                implementation: nameof(AiClientMock),
                targetSystem: "AI",
                reason: "初期 Mock，基于规则与随机数生成可解释结果");
        }

        /// <inheritdoc />
        public Task<PriceResponse> GetPriceAsync(PriceRequest request, CancellationToken cancellationToken = default)
        {
            var start = DateTimeOffset.UtcNow;
            var margin = request.TargetMargin ?? 0.15m;
            var rand = (decimal)(Random.Shared.NextDouble() * 0.05);
            var price = Math.Round(request.Cost * (1 + margin + rand), 2);

            var resp = new PriceResponse
            {
                Sku = request.Sku,
                RecommendedPrice = price,
                Currency = request.Currency,
                Rationale = $"成本={request.Cost}，目标毛利={margin:P0}，扰动={rand:P1}",
                GeneratedAt = DateTimeOffset.UtcNow
            };

            _audit.LogExternalCall(
                system: "AI",
                action: "Price",
                url: "mock://ai/price",
                statusCode: 200,
                duration: DateTimeOffset.UtcNow - start,
                success: true,
                requestBody: $"sku={request.Sku},cost={request.Cost},margin={request.TargetMargin}",
                responseBody: $"price={resp.RecommendedPrice}");

            return Task.FromResult(resp);
        }

        /// <inheritdoc />
        public Task<ForecastResponse> ForecastAsync(ForecastRequest request, CancellationToken cancellationToken = default)
        {
            var start = DateTimeOffset.UtcNow;
            var baseUnits = 100 + Random.Shared.Next(0, 50);
            var expected = baseUnits * Math.Max(1, request.HorizonWeeks);

            var resp = new ForecastResponse
            {
                Sku = request.Sku,
                ExpectedUnits = expected,
                GeneratedAt = DateTimeOffset.UtcNow
            };

            _audit.LogExternalCall(
                system: "AI",
                action: "Forecast",
                url: "mock://ai/forecast",
                statusCode: 200,
                duration: DateTimeOffset.UtcNow - start,
                success: true,
                requestBody: $"sku={request.Sku},weeks={request.HorizonWeeks}",
                responseBody: $"units={resp.ExpectedUnits}");

            return Task.FromResult(resp);
        }

        /// <inheritdoc />
        public Task<CreditResponse> CreditAssessAsync(CreditRequest request, CancellationToken cancellationToken = default)
        {
            var start = DateTimeOffset.UtcNow;
            var score = 60 + Random.Shared.Next(0, 41); // 60-100
            var limit = Math.Round(request.RequestedAmount * (0.5m + (decimal)score / 200m), 2);

            var resp = new CreditResponse
            {
                CustomerId = request.CustomerId,
                Score = score,
                SuggestedLimit = limit,
                Advice = score >= 80 ? "建议通过" : (score >= 70 ? "建议人工复核" : "建议谨慎"),
                GeneratedAt = DateTimeOffset.UtcNow
            };

            _audit.LogExternalCall(
                system: "AI",
                action: "Credit",
                url: "mock://ai/credit",
                statusCode: 200,
                duration: DateTimeOffset.UtcNow - start,
                success: true,
                requestBody: $"cust={request.CustomerId},amt={request.RequestedAmount}",
                responseBody: $"score={resp.Score},limit={resp.SuggestedLimit}");

            return Task.FromResult(resp);
        }
    }
}

