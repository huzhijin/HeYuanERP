using System;

namespace HeYuanERP.Domain.Invoices;

/// <summary>
/// 发票状态（仅用于领域层，不涉及持久化细节）。
/// </summary>
public enum InvoiceStatus
{
    /// <summary>
    /// 草稿（可编辑、未正式开具）。
    /// </summary>
    Draft = 0,

    /// <summary>
    /// 待开具（已生成待审核/待开票）。
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 已开具（发票号码/代码已生成，允许打印）。
    /// </summary>
    Issued = 2,

    /// <summary>
    /// 已作废（开具后作废，不可再打印）。
    /// </summary>
    Canceled = 3
}

