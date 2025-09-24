using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using HeYuanERP.Application.OA;
using HeYuanERP.Infrastructure.Clients.Oa;
using HeYuanERP.Infrastructure.Logging.Audit;
using Xunit;

namespace HeYuanERP_Server.Tests.Clients
{
    /// <summary>
    /// OA Mock 客户端基础单元测试（不依赖外部网络/服务）。
    /// </summary>
    public class OaClientMockTests
    {
        private sealed class FakeAuditLogger : IAuditLogger
        {
            public ConcurrentBag<(string System, string Action, string Url, int? Status, TimeSpan Duration, bool Success)> Entries { get; } = new();

            public void LogExternalCall(string system, string action, string url, int? statusCode, TimeSpan duration, bool success, string? requestBody = null, string? responseBody = null, string? traceId = null)
            {
                Entries.Add((system, action, url, statusCode, duration, success));
            }

            public void LogReplacementNotice(string component, string implementation, string? targetSystem = null, string? reason = null)
            {
                // 记录替换说明，不做断言使用
            }
        }

        [Fact(DisplayName = "SSO 登录返回令牌与过期时间（Mock）")]
        public async Task SsoLogin_ReturnsToken()
        {
            var audit = new FakeAuditLogger();
            var client = new OaClientMock(audit);

            var resp = await client.SsoLoginAsync(new OaSsoLoginRequest { UserCode = "U001" });

            Assert.False(string.IsNullOrWhiteSpace(resp.AccessToken));
            Assert.True(resp.ExpiresAt > DateTimeOffset.UtcNow);
            Assert.Contains(audit.Entries, e => e.System == "OA" && e.Action == "SSO.Login" && e.Success);
        }

        [Fact(DisplayName = "待办查询分页与过滤（Mock）")]
        public async Task GetTodos_PagingWorks()
        {
            var audit = new FakeAuditLogger();
            var client = new OaClientMock(audit);

            var result = await client.GetTodosAsync(new OaTodoQuery { Page = 2, PageSize = 5, Status = "Pending" });

            Assert.Equal(2, result.Page);
            Assert.Equal(5, result.PageSize);
            Assert.True(result.Total >= result.Items.Count());
            Assert.All(result.Items, i => Assert.Equal("Pending", i.Status));
            Assert.Contains(audit.Entries, e => e.Action == "Todo.Query" && e.Success);
        }

        [Fact(DisplayName = "回写成功（Mock）")]
        public async Task Writeback_Succeeds()
        {
            var audit = new FakeAuditLogger();
            var client = new OaClientMock(audit);

            var resp = await client.WritebackAsync(new OaWritebackRequest
            {
                BizType = "Sales",
                BizId = "B001",
                Status = "Approved",
                UserCode = "U001"
            });

            Assert.True(resp.Success);
            Assert.Contains(audit.Entries, e => e.Action == "Writeback" && e.Success);
        }

        [Fact(DisplayName = "附件上传与下载（Mock）")]
        public async Task UploadAndDownload_Works()
        {
            var audit = new FakeAuditLogger();
            var client = new OaClientMock(audit);

            var up = await client.UploadAttachmentAsync(new OaAttachmentUploadRequest
            {
                FileName = "demo.txt",
                ContentType = "text/plain",
                Content = System.Text.Encoding.UTF8.GetBytes("hello")
            });

            Assert.False(string.IsNullOrWhiteSpace(up.AttachmentId));
            Assert.False(string.IsNullOrWhiteSpace(up.Url));

            var down = await client.DownloadAttachmentAsync(up.AttachmentId);
            Assert.Equal(up.AttachmentId + ".txt", down.FileName);
            Assert.NotNull(down.Content);
            Assert.True(down.Content.Length > 0);

            Assert.Contains(audit.Entries, e => e.Action == "Attachment.Upload" && e.Success);
            Assert.Contains(audit.Entries, e => e.Action == "Attachment.Download" && e.Success);
        }
    }
}

