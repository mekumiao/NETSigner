using System.Diagnostics.CodeAnalysis;

namespace NETSigner;

public class SignatureHeader
{
    public long Timestamp { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public string Signature { get; private set; } = string.Empty;
    public string Nonce { get; private set; } = string.Empty;
    public string? SignatureMethod { get; private set; } = string.Empty;

    protected SignatureHeader()
    {
    }

    public static bool TryParse(IDictionary<string, string?> headers, VerifyResult result, [NotNullWhen(true)] out SignatureHeader? signatureHeader)
    {
        signatureHeader = null;
        var header = new SignatureHeader();

        if (headers.TryGetValue(SignatureConstant.XCaTimestamp, out var timestampString))
        {
            if (long.TryParse(timestampString, out var timestamp))
            {
                header.Timestamp = timestamp;
            }
            else
            {
                result.ErrorMessage = $"Invalid parameter, The {SignatureConstant.XCaTimestamp} is Invalid";
                return false;
            }
        }
        else
        {
            result.ErrorMessage = $"Missing parameter, The {SignatureConstant.XCaTimestamp} is required";
            return false;
        }

        if (!RequiredHeader(headers, SignatureConstant.XCaKey, result, x => header.Key = x))
        {
            return false;
        }

        if (!RequiredHeader(headers, SignatureConstant.XCaSignature, result, x => header.Signature = x))
        {
            return false;
        }

        if (!RequiredHeader(headers, SignatureConstant.XCaNonce, result, x => header.Nonce = x))
        {
            return false;
        }

        if (headers.TryGetValue(SignatureConstant.XCaSignatureMethod, out var method))
        {
            header.SignatureMethod = method;
        }

        signatureHeader = header;
        return true;
    }

    private static bool RequiredHeader(IDictionary<string, string?> headers, string key, VerifyResult result, Action<string> action)
    {
        if (headers.TryGetValue(key, out var value))
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                action(value);
                return true;
            }
            else
            {
                result.ErrorMessage = $"Invalid parameter, The {key} is Invalid";
                return false;
            }
        }
        else
        {
            result.ErrorMessage = $"Missing parameter, The {key} is required";
            return false;
        }
    }
}
