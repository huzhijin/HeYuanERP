using System.Security.Cryptography;
using System.Text;

namespace HeYuanERP.Api.Services;

/// <summary>
/// 密码哈希服务：从环境变量读取算法与 Pepper，并提供 Hash/Verify 方法。
/// 环境变量：
/// - PASSWORD__ALGO：默认 SHA256（可选 PLAINTEXT 仅用于开发）
/// - PASSWORD__PEPPER：系统级 Pepper（建议较长随机串）
/// </summary>
public class PasswordHasher
{
    private readonly string _algo;
    private readonly string _pepper;

    public PasswordHasher()
    {
        _algo = Environment.GetEnvironmentVariable("PASSWORD__ALGO") ?? "SHA256";
        _pepper = Environment.GetEnvironmentVariable("PASSWORD__PEPPER") ?? string.Empty;
    }

    /// <summary>
    /// 计算密码哈希值。
    /// SHA256：对 (pepper + password) 进行哈希，输出十六进制小写。
    /// </summary>
    public string Hash(string password)
    {
        if (string.Equals(_algo, "PLAINTEXT", StringComparison.OrdinalIgnoreCase))
        {
            return password;
        }

        if (string.Equals(_algo, "SHA256", StringComparison.OrdinalIgnoreCase))
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(_pepper + password));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        // 未知算法：回退为 SHA256
        using (var sha = SHA256.Create())
        {
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(_pepper + password));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }

    /// <summary>
    /// 校验密码是否匹配存储哈希。
    /// 兼容开发期：若配置为 PLAINTEXT 或存储值与明文相等也视为通过。
    /// </summary>
    public bool Verify(string storedHash, string password)
    {
        if (string.IsNullOrEmpty(storedHash)) return false;

        // 开发期兼容：明文
        if (string.Equals(_algo, "PLAINTEXT", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(storedHash, password, StringComparison.Ordinal);
        }

        var calc = Hash(password);
        if (string.Equals(storedHash, calc, StringComparison.Ordinal))
            return true;

        // 临时回退：若库内意外保存了明文，允许在开发期通过
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
        var isDev = env.Equals("Development", StringComparison.OrdinalIgnoreCase);
        if (isDev && string.Equals(storedHash, password, StringComparison.Ordinal))
            return true;

        return false;
    }
}

