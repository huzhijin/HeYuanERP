using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Application.OA;
using HeYuanERP.Infrastructure.Logging.Audit;

namespace HeYuanERP.Infrastructure.Clients.Oa
{
    /// <summary>
    /// OA 客户端 Mock 实现（用于初期无真实 OA 系统时的替身）。
    /// 特性：纯内存模拟，快速/稳定，不调用外部网络，不影响主链路。
    /// </summary>
    public class OaClientMock : IOaClient
    {
        private readonly IAuditLogger _audit;

        public OaClientMock(IAuditLogger auditLogger)
        {
            _audit = auditLogger;
            _audit.LogReplacementNotice(
                component: nameof(IOaClient),
                implementation: nameof(OaClientMock),
                targetSystem: "OA",
                reason: "初期 Mock，按 OpenAPI 契约联调占位，不影响主链路");
        }

        /// <inheritdoc />
        public Task<OaSsoLoginResponse> SsoLoginAsync(OaSsoLoginRequest request, CancellationToken cancellationToken = default)
        {
            var start = DateTimeOffset.UtcNow;
            var resp = new OaSsoLoginResponse
            {
                AccessToken = $"mock-token-{Guid.NewGuid():N}",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(1),
                RedirectUrl = "https://mock.oa.local/portal"
            };

            _audit.LogExternalCall(
                system: "OA",
                action: "SSO.Login",
                url: "mock://oa/sso/login",
                statusCode: 200,
                duration: DateTimeOffset.UtcNow - start,
                success: true,
                requestBody: $"user={request.UserCode}",
                responseBody: "{token: mock}");

            return Task.FromResult(resp);
        }

        /// <inheritdoc />
        public Task<PagedResult<OaTodoItem>> GetTodosAsync(OaTodoQuery query, CancellationToken cancellationToken = default)
        {
            var start = DateTimeOffset.UtcNow;

            // 生成 50 条模拟待办数据
            var all = Enumerable.Range(1, 50).Select(i => new OaTodoItem
            {
                Id = $"TD{i:D4}",
                Title = $"待办事项 {i}",
                BizType = (i % 2 == 0) ? "Purchase" : "Sales",
                BizId = $"BIZ{i:D6}",
                Status = (i % 3 == 0) ? "Done" : "Pending",
                CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-i * 3),
                Url = $"https://mock.oa.local/todo/{i}"
            });

            if (!string.IsNullOrWhiteSpace(query.Status))
                all = all.Where(t => string.Equals(t.Status, query.Status, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(query.Keyword))
                all = all.Where(t => t.Title.Contains(query.Keyword!, StringComparison.OrdinalIgnoreCase));

            var total = all.LongCount();
            var page = Math.Max(1, query.Page);
            var pageSize = Math.Max(1, query.PageSize);
            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToArray();

            var result = new PagedResult<OaTodoItem>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items
            };

            _audit.LogExternalCall(
                system: "OA",
                action: "Todo.Query",
                url: "mock://oa/todos",
                statusCode: 200,
                duration: DateTimeOffset.UtcNow - start,
                success: true,
                requestBody: $"page={page}&size={pageSize}&status={query.Status}&kw={query.Keyword}",
                responseBody: $"count={items.Length}/total={total}");

            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<OaWritebackResponse> WritebackAsync(OaWritebackRequest request, CancellationToken cancellationToken = default)
        {
            var start = DateTimeOffset.UtcNow;

            // Mock：认为所有回写均成功
            var resp = new OaWritebackResponse
            {
                Success = true,
                Message = "回写成功（Mock）"
            };

            _audit.LogExternalCall(
                system: "OA",
                action: "Writeback",
                url: "mock://oa/writeback",
                statusCode: 200,
                duration: DateTimeOffset.UtcNow - start,
                success: true,
                requestBody: $"biz={request.BizType}/{request.BizId},status={request.Status}",
                responseBody: "ok");

            return Task.FromResult(resp);
        }

        /// <inheritdoc />
        public Task<OaAttachmentUploadResponse> UploadAttachmentAsync(OaAttachmentUploadRequest request, CancellationToken cancellationToken = default)
        {
            var start = DateTimeOffset.UtcNow;
            var id = $"ATT-{Guid.NewGuid():N}";
            var resp = new OaAttachmentUploadResponse
            {
                AttachmentId = id,
                Url = $"https://mock.oa.local/attachments/{id}"
            };

            _audit.LogExternalCall(
                system: "OA",
                action: "Attachment.Upload",
                url: "mock://oa/attachments",
                statusCode: 200,
                duration: DateTimeOffset.UtcNow - start,
                success: true,
                requestBody: $"file={request.FileName},size={request.Content?.Length ?? 0}",
                responseBody: $"id={id}");

            return Task.FromResult(resp);
        }

        /// <inheritdoc />
        public Task<OaAttachmentDownloadResponse> DownloadAttachmentAsync(string attachmentId, CancellationToken cancellationToken = default)
        {
            var start = DateTimeOffset.UtcNow;
            var content = System.Text.Encoding.UTF8.GetBytes($"Mock-Content for {attachmentId} @ {DateTimeOffset.UtcNow:O}");
            var resp = new OaAttachmentDownloadResponse
            {
                FileName = $"{attachmentId}.txt",
                ContentType = "text/plain; charset=utf-8",
                Content = content
            };

            _audit.LogExternalCall(
                system: "OA",
                action: "Attachment.Download",
                url: $"mock://oa/attachments/{attachmentId}",
                statusCode: 200,
                duration: DateTimeOffset.UtcNow - start,
                success: true,
                requestBody: null,
                responseBody: $"bytes={content.Length}");

            return Task.FromResult(resp);
        }
    }
}

