using System.Threading;
using System.Threading.Tasks;

namespace HeYuanERP.Application.OA
{
    /// <summary>
    /// OA 外部系统客户端接口（SSO/待办/回写/附件）。
    /// 说明：仅定义契约，具体实现由基础设施层提供（初期为 Mock，不影响主链路）。
    /// </summary>
    public interface IOaClient
    {
        /// <summary>
        /// SSO 单点登录，返回授权票据或会话信息。
        /// </summary>
        /// <param name="request">登录请求参数</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<OaSsoLoginResponse> SsoLoginAsync(OaSsoLoginRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取待办列表（分页）。
        /// </summary>
        /// <param name="query">查询条件（含分页）</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<PagedResult<OaTodoItem>> GetTodosAsync(OaTodoQuery query, CancellationToken cancellationToken = default);

        /// <summary>
        /// 回写业务状态到 OA。
        /// </summary>
        /// <param name="request">回写请求</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<OaWritebackResponse> WritebackAsync(OaWritebackRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// 上传附件到 OA。
        /// </summary>
        /// <param name="request">上传请求（含文件内容/路径等）</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<OaAttachmentUploadResponse> UploadAttachmentAsync(OaAttachmentUploadRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// 下载附件。
        /// </summary>
        /// <param name="attachmentId">附件标识</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<OaAttachmentDownloadResponse> DownloadAttachmentAsync(string attachmentId, CancellationToken cancellationToken = default);
    }

    // 说明：以下 DTO 类型与分页模型将在后续批次提供（OaDtos.cs），本文件仅引用类型名称以稳定接口。
}

