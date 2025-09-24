using System;
using System.Collections.Generic;

namespace HeYuanERP.Application.OA
{
    /// <summary>
    /// 通用分页结果模型（用于 OA 待办列表等）。
    /// </summary>
    public class PagedResult<T>
    {
        /// <summary>
        /// 当前页码（从 1 开始）。
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 每页大小。
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总记录数。
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 记录集合。
        /// </summary>
        public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
    }

    /// <summary>
    /// SSO 登录请求。
    /// </summary>
    public class OaSsoLoginRequest
    {
        /// <summary>
        /// 用户编码（AD/工号）。
        /// </summary>
        public string UserCode { get; set; } = string.Empty;

        /// <summary>
        /// 密码/凭证（根据实际接入可为空）。
        /// </summary>
        public string? Credential { get; set; }
    }

    /// <summary>
    /// SSO 登录响应。
    /// </summary>
    public class OaSsoLoginResponse
    {
        /// <summary>
        /// 访问令牌（Mock）。
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 令牌过期时间。
        /// </summary>
        public DateTimeOffset ExpiresAt { get; set; }

        /// <summary>
        /// 登录后跳转地址（如 OA 门户）。
        /// </summary>
        public string? RedirectUrl { get; set; }
    }

    /// <summary>
    /// 待办查询条件（含分页）。
    /// </summary>
    public class OaTodoQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? UserCode { get; set; }
        public string? Status { get; set; }
        public string? Keyword { get; set; }
    }

    /// <summary>
    /// 待办项。
    /// </summary>
    public class OaTodoItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string BizType { get; set; } = string.Empty;
        public string BizId { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTimeOffset CreatedAt { get; set; }
        public string Url { get; set; } = string.Empty;
    }

    /// <summary>
    /// 回写请求（将 ERP 审批/状态回写至 OA）。
    /// </summary>
    public class OaWritebackRequest
    {
        public string BizType { get; set; } = string.Empty;
        public string BizId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public string? Comment { get; set; }
    }

    /// <summary>
    /// 回写响应。
    /// </summary>
    public class OaWritebackResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// 附件上传请求。
    /// </summary>
    public class OaAttachmentUploadRequest
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string BizType { get; set; } = string.Empty;
        public string BizId { get; set; } = string.Empty;
    }

    /// <summary>
    /// 附件上传响应。
    /// </summary>
    public class OaAttachmentUploadResponse
    {
        public string AttachmentId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    /// <summary>
    /// 附件下载响应。
    /// </summary>
    public class OaAttachmentDownloadResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/octet-stream";
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}

