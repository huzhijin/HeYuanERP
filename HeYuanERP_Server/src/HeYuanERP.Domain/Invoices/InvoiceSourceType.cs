using System;

namespace HeYuanERP.Domain.Invoices;

/// <summary>
/// 发票来源类型（用于标识发票由何种业务单据生成）。
/// </summary>
public enum InvoiceSourceType
{
    /// <summary>
    /// 来源于销售订单。
    /// </summary>
    SalesOrder = 0,

    /// <summary>
    /// 来源于交货/发运单。
    /// </summary>
    Delivery = 1
}

