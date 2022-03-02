using System.Diagnostics.CodeAnalysis;

namespace NETSigner;

/// <summary>
/// 签名文本生成器
/// </summary>
public class SignatureTextGenerator
{
    public bool TryGetSignatureText(HttpRequestModel model, VerifyResult result, [NotNullWhen(true)] out SignatureText? signatureText)
    {
        signatureText = null;

        if (!model.Headers.ContainsKey(SignatureConstant.XCaKey))
        {
            result.ErrorMessage = $"Missing parameter, The {SignatureConstant.XCaKey} is required";
            return false;
        }

        if (!model.Headers.ContainsKey(SignatureConstant.XCaTimestamp))
        {
            result.ErrorMessage = $"Missing parameter, The {SignatureConstant.XCaTimestamp} is required";
            return false;
        }

        if (!model.Headers.ContainsKey(SignatureConstant.XCaNonce))
        {
            result.ErrorMessage = $"Missing parameter, The {SignatureConstant.XCaNonce} is required";
            return false;
        }

        var text = new SignatureText
        {
            Path = model.Path,
            Form = model.Form,
            Query = model.Query,
            HTTPMethod = model.Method
        };

        if (model.Headers.TryGetValue(SignatureConstant.ContentMD5, out var md5))
        {
            text.ContentMD5 = md5;
        }

        if (model.Headers.TryGetValue(SignatureConstant.ContentType, out var type))
        {
            text.ContentType = type;
        }

        if (model.Headers.TryGetValue(SignatureConstant.Date, out var date))
        {
            text.Date = date;
        }

        if (model.Headers.TryGetValue(SignatureConstant.Accept, out var accept))
        {
            text.Accept = accept;
        }

        if (model.Headers.TryGetValue(SignatureConstant.XCaSignatureHeaders, out var signatureHeaders))
        {
            text.SignatureHeaders = signatureHeaders?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToHashSet(StringComparer.OrdinalIgnoreCase);

            text.SignatureHeaders?.Remove(SignatureConstant.XCaSignature);
            text.SignatureHeaders?.Remove(SignatureConstant.XCaSignatureHeaders);
            text.SignatureHeaders?.Remove(SignatureConstant.Accept);
            text.SignatureHeaders?.Remove(SignatureConstant.ContentMD5);
            text.SignatureHeaders?.Remove(SignatureConstant.ContentType);
            text.SignatureHeaders?.Remove(SignatureConstant.Date);
        }

        if (text.SignatureHeaders == null || !text.SignatureHeaders.Any())
        {
            text.SignatureHeaders = new()
            {
                SignatureConstant.XCaKey,
                SignatureConstant.XCaTimestamp,
                SignatureConstant.XCaNonce,
            };
        }
        text.Headers = model.Headers.Join(text.SignatureHeaders,
                                                     t1 => t1.Key,
                                                     t2 => t2,
                                                     (t1, t2) => t1,
                                                     StringComparer.OrdinalIgnoreCase)
                                               .ToDictionary(x => x.Key, y => y.Value);

        signatureText = text;
        return true;
    }
}
