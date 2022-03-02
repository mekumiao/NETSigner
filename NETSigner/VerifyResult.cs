namespace NETSigner;

public class VerifyResult
{
    public string ErrorMessage { get; set; } = string.Empty;
    public bool IsSuccess => ErrorMessage == string.Empty;
}
