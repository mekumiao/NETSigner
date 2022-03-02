namespace NETSigner;

public class SignatureHeader
{
    public bool IsSuccess => ErrorMessage == string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public long Timestamp { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public string Nonce { get; set; } = string.Empty;
    public string? SignatureMethod { get; set; } = string.Empty;
}
