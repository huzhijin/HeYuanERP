using System;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HeYuanERP.Api.Swagger;

/// <summary>
/// Swagger Operation 过滤器：
/// 为打印接口（api/print/...）声明 200 响应为 application/pdf（二进制流）。
/// </summary>
public class ProducesPdfOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var path = context.ApiDescription.RelativePath ?? string.Empty;
        var isPrintApi = path.StartsWith("api/print", StringComparison.OrdinalIgnoreCase)
                         || HasProducesPdfAttribute(context);

        if (!isPrintApi)
        {
            return;
        }

        var response = operation.Responses.ContainsKey("200")
            ? operation.Responses["200"]
            : new OpenApiResponse { Description = "OK" };

        // 以二进制流返回 PDF
        response.Content.Clear();
        response.Content[MediaTypeNames.Application.Pdf] = new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Type = "string",
                Format = "binary"
            }
        };

        operation.Responses["200"] = response;
    }

    private static bool HasProducesPdfAttribute(OperationFilterContext context)
    {
        var producesAttrs = context.MethodInfo.GetCustomAttributes(true)
            .OfType<ProducesAttribute>()
            .SelectMany(a => a.ContentTypes)
            .Select(ct => ct)
            .ToArray();

        return producesAttrs.Any(ct => string.Equals(ct, MediaTypeNames.Application.Pdf, StringComparison.OrdinalIgnoreCase));
    }
}
