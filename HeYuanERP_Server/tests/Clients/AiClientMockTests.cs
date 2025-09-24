using System.Collections.Concurrent;
using System;
using System.Threading.Tasks;
using HeYuanERP.Application.AI;
using HeYuanERP.Infrastructure.Clients.Ai;
using HeYuanERP.Infrastructure.Logging.Audit;
using Xunit;

namespace HeYuanERP_Server.Tests.Clients
{
    /// <summary>
    /// AI Mock 客户端基础单元测试。
    /// </summary>
    public class AiClientMockTests
    {
        private sealed class FakeAuditLogger : IAuditLogger
        {
            public ConcurrentBag<(string System, string Action, string Url, int? Status, TimeSpan Duration, bool Success)> Entries { get; } = new();

            public void LogExternalCall(string system, string action, string url, int? statusCode, TimeSpan duration, bool success, string? requestBody = null, string? responseBody = null, string? traceId = null)
                => Entries.Add((system, action, url, statusCode, duration, success));

            public void LogReplacementNotice(string component, string implementation, string? targetSystem = null, string? reason = null)
            {
            }
        }

        [Fact(DisplayName = "定价：价格高于成本，含理由（Mock）")]
        public async Task Price_GeneratesReasonable()
        {
            var audit = new FakeAuditLogger();
            var client = new AiClientMock(audit);

            var resp = await client.GetPriceAsync(new PriceRequest { Sku = "SKU001", Cost = 10m, TargetMargin = 0.2m });
            Assert.Equal("SKU001", resp.Sku);
            Assert.True(resp.RecommendedPrice > 10m);
            Assert.False(string.IsNullOrWhiteSpace(resp.Rationale));
            Assert.Contains(audit.Entries, e => e.Action == "Price" && e.Success);
        }

        [Fact(DisplayName = "预测：返回正销量（Mock）")]
        public async Task Forecast_PositiveUnits()
        {
            var audit = new FakeAuditLogger();
            var client = new AiClientMock(audit);

            var resp = await client.ForecastAsync(new ForecastRequest { Sku = "SKU002", HorizonWeeks = 6 });
            Assert.Equal("SKU002", resp.Sku);
            Assert.True(resp.ExpectedUnits > 0);
            Assert.Contains(audit.Entries, e => e.Action == "Forecast" && e.Success);
        }

        [Fact(DisplayName = "信用：得分范围与建议（Mock）")]
        public async Task Credit_ScoreAndAdvice()
        {
            var audit = new FakeAuditLogger();
            var client = new AiClientMock(audit);

            var resp = await client.CreditAssessAsync(new CreditRequest { CustomerId = "C001", RequestedAmount = 50000m });
            Assert.Equal("C001", resp.CustomerId);
            Assert.InRange(resp.Score, 60, 100);
            Assert.True(resp.SuggestedLimit > 0);
            Assert.False(string.IsNullOrWhiteSpace(resp.Advice));
            Assert.Contains(audit.Entries, e => e.Action == "Credit" && e.Success);
        }
    }
}

