using System.Security.Cryptography;
using System.Text;

namespace NETSigner;

/// <summary>
/// 签名生成器
/// </summary>
public class HmacSHA256SignatureGenerator : ISignatureGenerator
{
    public string Name => "HmacSHA256";

    public string Signature(string sk, string plainText)
    {
        var keyBytes = Encoding.UTF8.GetBytes(sk);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        using var sha256 = new HMACSHA256(keyBytes);
        var hashBytes = sha256.ComputeHash(plainBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
