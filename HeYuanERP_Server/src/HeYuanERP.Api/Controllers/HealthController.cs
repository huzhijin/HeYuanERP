using HeYuanERP.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeYuanERP.Api.Controllers;

// 健康检查控制器：/healthz 用于本地与监控探活
[ApiController]
[Route("healthz")]
public class HealthController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Get()
    {
        // 返回统一响应，包含版本与时间戳
        var data = new
        {
            service = "HeYuanERP.Api",
            version = typeof(HealthController).Assembly.GetName().Version?.ToString() ?? "1.0.0",
            time = DateTime.UtcNow
        };
        return Ok(ApiResponse<object>.Ok(data));
    }
}

