using System.Threading;
using System.Threading.Tasks;
using HeYuanERP.Domain.Printing.Models;

namespace HeYuanERP.Application.Printing;

/// <summary>
/// 模板参数快照存储接口：用于保存与读取打印时的视图模型快照，保证可重现性。
/// 示例用途：审计回溯、二次打印一致性等。
/// </summary>
public interface IPrintSnapshotStore
{
    /// <summary>
    /// 保存快照（若存在则覆盖或根据实现策略写入新版本）。
    /// </summary>
    /// <param name="snapshot">快照对象。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task SaveAsync(PrintSnapshot snapshot, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已有快照（若不存在返回 null）。
    /// </summary>
    /// <param name="docType">单据类型。</param>
    /// <param name="id">单据标识。</param>
    /// <param name="templateName">模板名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>快照或 null。</returns>
    Task<PrintSnapshot?> TryGetAsync(string docType, string id, string templateName, CancellationToken cancellationToken = default);
}

