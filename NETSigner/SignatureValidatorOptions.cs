namespace NETSigner;

public class SignatureValidatorOptions
{
    /// <summary>
    /// 时间戳验证偏移量. default is 5 seconds
    /// </summary>
    public TimeSpan TimestampValidationOffset { get; set; } = TimeSpan.FromSeconds(5);
    /// <summary>
    /// 是否验证时间戳. default is true
    /// </summary>
    public bool IsValidateTimestamp { get; set; } = true;
    /// <summary>
    /// 是否验证Nonce. default is true
    /// </summary>
    public bool IsValidateNonce { get; set; } = true;
    /// <summary>
    /// 签名算法
    /// </summary>
    public string SignatureMethod { get; set; } = "HmacSHA256";
}
