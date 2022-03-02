namespace NETSigner;

public class SignatureValidatorOptions
{
    /// <summary>
    /// 时间戳验证偏移量
    /// </summary>
    public TimeSpan TimestampValidationOffset { get; set; } = TimeSpan.FromSeconds(5);
    /// <summary>
    /// 签名算法
    /// </summary>
    public string SignatureMethod { get; set; } = "HmacSHA256";
}
