using System.Diagnostics.CodeAnalysis;

namespace NETSigner;

/// <summary>
/// 签名文本生成器
/// </summary>
public class SignatureHeaderGenerator
{
    public bool TryGetSignatureHeader(HttpRequestModel model, VerifyResult result, [NotNullWhen(true)] out SignatureHeader? signatureHeader)
    {
        signatureHeader = null;
        var header = new SignatureHeader();

        if (model.Headers.TryGetValue(SignatureConstant.XCaTimestamp, out var timestampString))
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

        if (!RequiredHeader(model.Headers, SignatureConstant.XCaKey, result, x => header.Key = x))
        {
            return false;
        }

        if (!RequiredHeader(model.Headers, SignatureConstant.XCaSignature, result, x => header.Signature = x))
        {
            return false;
        }

        if (!RequiredHeader(model.Headers, SignatureConstant.XCaNonce, result, x => header.Nonce = x))
        {
            return false;
        }

        if (model.Headers.TryGetValue(SignatureConstant.XCaSignatureMethod, out var method))
        {
            header.SignatureMethod = method;
        }

        signatureHeader = header;
        return true;
    }

    private bool RequiredHeader(IDictionary<string, string?> headers, string key, VerifyResult result, Action<string> action)
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
