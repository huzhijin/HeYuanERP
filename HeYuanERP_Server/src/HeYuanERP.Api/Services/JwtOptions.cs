namespace HeYuanERP.Api.Services;

// JWT 配置项（从环境变量注入）
public class JwtOptions
{
    public string Issuer { get; }
    public string Audience { get; }
    public string Secret { get; }
    public int ExpireMinutes { get; }

    public JwtOptions(string issuer, string audience, string secret, int expireMinutes = 120)
    {
        Issuer = issuer;
        Audience = audience;
        Secret = secret;
        ExpireMinutes = expireMinutes;
    }
}

