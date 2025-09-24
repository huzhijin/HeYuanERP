using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Services.Orders;

// 特价/项目申请留桩：
// - 仅做占位，不落库；返回一个临时申请号与固定状态。
// - 真实实现将接入 OA/AI 审批流，并记录审计/附件等。
public static class SpecialPriceStub
{
    // 生成占位申请号：SPyyyyMMdd-HHmmss-随机四位
    public static string NewApplyId()
    {
        var ts = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var rnd = Random.Shared.Next(0, 9999).ToString("D4");
        return $"SP{ts}-{rnd}";
    }

    // 构造提交结果（固定返回 submitted/pending）
    public static OrderSpecialApplyResultDto BuildSubmittedResult(OrderSpecialApplyDto req)
    {
        return new OrderSpecialApplyResultDto
        {
            ApplyId = NewApplyId(),
            Status = "submitted",
            Message = "已接收，后续由 OA/AI 审批流程处理（Mock）",
            Echo = req
        };
    }
}

