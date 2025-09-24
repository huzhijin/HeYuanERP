using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace HeYuanERP.Application.Printing;

/// <summary>
/// 打印客户端实现（HTTP 调用 P11 渲染服务）。
/// 所有连接参数支持从环境变量读取：
/// - PRINTING_BASE_URL（必需）：P11 服务基础地址
/// - PRINTING_API_KEY（可选）：P11 服务鉴权密钥
/// - PRINTING_RENDER_PATH（可选，默认 /api/print/render）：渲染接口路径
/// - PRINTING_TIMEOUT_SECONDS（可选，默认 60）
/// </summary>
public class PrintClient : IPrintClient, IDisposable
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private readonly string? _apiKey;
    private readonly string _renderPath;
    private bool _disposeClient;

    public PrintClient(HttpClient httpClient, string baseUrl, string? apiKey = null, string renderPath = "/api/print/render")
    {
        _http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _baseUrl = string.IsNullOrWhiteSpace(baseUrl) ? throw new ArgumentNullException(nameof(baseUrl)) : baseUrl.TrimEnd('/');
        _apiKey = apiKey;
        _renderPath = string.IsNullOrWhiteSpace(renderPath) ? "/api/print/render" : renderPath;
    }

    /// <summary>
    /// 使用环境变量快速构造客户端。
    /// </summary>
    public PrintClient()
    {
        var baseUrl = Environment.GetEnvironmentVariable("PRINTING_BASE_URL") ?? string.Empty;
        var apiKey = Environment.GetEnvironmentVariable("PRINTING_API_KEY");
        var renderPath = Environment.GetEnvironmentVariable("PRINTING_RENDER_PATH") ?? "/api/print/render";
        var timeoutSecondsStr = Environment.GetEnvironmentVariable("PRINTING_TIMEOUT_SECONDS");

        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("未配置 PRINTING_BASE_URL 环境变量");

        var handler = new HttpClientHandler();
        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(int.TryParse(timeoutSecondsStr, out var t) ? Math.Clamp(t, 5, 300) : 60)
        };

        _http = client;
        _disposeClient = true;
        _baseUrl = baseUrl.TrimEnd('/');
        _apiKey = apiKey;
        _renderPath = string.IsNullOrWhiteSpace(renderPath) ? "/api/print/render" : renderPath;
    }

    public async Task<byte[]> RenderPdfAsync(string templateCode, object model, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(templateCode))
            throw new ArgumentNullException(nameof(templateCode));
        if (model == null) throw new ArgumentNullException(nameof(model));

        var url = new Uri(new Uri(_baseUrl + "/"), _renderPath.TrimStart('/'));

        var reqObj = new RenderRequest
        {
            TemplateCode = templateCode,
            Format = "pdf",
            Data = model
        };

        var json = JsonSerializer.Serialize(reqObj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/pdf"));
        if (!string.IsNullOrWhiteSpace(_apiKey))
        {
            request.Headers.TryAddWithoutValidation("X-API-KEY", _apiKey);
        }

        using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        var contentType = response.Content.Headers.ContentType?.MediaType?.ToLowerInvariant();
        if (contentType == "application/pdf")
        {
            return await response.Content.ReadAsByteArrayAsync(ct);
        }

        // 兼容 JSON 返回 base64 内容（如 P11 返回 JSON 包装）
        var text = await response.Content.ReadAsStringAsync(ct);
        try
        {
            using var doc = JsonDocument.Parse(text);
            string? base64 = null;
            if (doc.RootElement.TryGetProperty("contentBase64", out var c1) && c1.ValueKind == JsonValueKind.String)
                base64 = c1.GetString();
            else if (doc.RootElement.TryGetProperty("content", out var c2) && c2.ValueKind == JsonValueKind.String)
                base64 = c2.GetString();

            if (!string.IsNullOrEmpty(base64))
            {
                return Convert.FromBase64String(base64!);
            }
        }
        catch
        {
            // 忽略 JSON 解析异常，转抛格式错误
        }

        throw new InvalidOperationException("打印服务返回的格式无法识别，期望 PDF 或含 base64 的 JSON 响应");
    }

    public void Dispose()
    {
        if (_disposeClient)
        {
            _http.Dispose();
        }
    }

    private sealed class RenderRequest
    {
        public string TemplateCode { get; set; } = string.Empty;
        public string Format { get; set; } = "pdf";
        public object Data { get; set; } = default!;
    }
}

