namespace NETSigner;

/// <summary>
/// 签名验证器
/// </summary>
public class SignatureValidator
{
    private readonly ISKGetter _sKGetter;
    private readonly INonceRecorder _nonceRecorder;
    private readonly SignatureTextGenerator _signatureTextGenerator;
    private readonly SignatureHeaderGenerator _signatureHeaderGenerator;
    private readonly SignatureGeneratorRegistry _signatureGeneratorRegistry;
    private readonly SignatureValidatorOptions _signatureValidatorOptions;

    public SignatureValidator(
        ISKGetter sKGetter,
        INonceRecorder nonceRecorder,
        SignatureTextGenerator signatureTextGenerator,
        SignatureHeaderGenerator signatureHeaderGenerator,
        SignatureGeneratorRegistry signatureGeneratorRegistry,
        SignatureValidatorOptions signatureValidatorOptions)
    {
        _sKGetter = sKGetter;
        _nonceRecorder = nonceRecorder;
        _signatureTextGenerator = signatureTextGenerator;
        _signatureHeaderGenerator = signatureHeaderGenerator;
        _signatureGeneratorRegistry = signatureGeneratorRegistry;
        _signatureValidatorOptions = signatureValidatorOptions;
    }

    public VerifyResult Verify(HttpRequestModel requestModel)
    {
        var result = new VerifyResult();
        if (_signatureHeaderGenerator.TryGetSignatureHeader(requestModel, result, out var signatureHeader))
        {
            if (_signatureTextGenerator.TryGetSignatureText(requestModel, result, out var signatureText))
            {
                _ = VerifyTimestamp(signatureHeader.Timestamp, result) && VerifyNonce(signatureHeader.Nonce, result) && VerifySignature(signatureHeader, signatureText, result);
            }
        }
        return result;
    }

    protected bool VerifyTimestamp(long timestamp, VerifyResult result)
    {
        if (_signatureValidatorOptions.IsValidateTimestamp)
        {
            var left = DateTimeOffset.Now.Add(-_signatureValidatorOptions.TimestampValidationOffset).ToUnixTimeMilliseconds();
            var right = DateTimeOffset.Now.Add(_signatureValidatorOptions.TimestampValidationOffset).ToUnixTimeMilliseconds();
            if (timestamp >= left && timestamp <= right)
            {
                return true;
            }
            result.ErrorMessage = "Invalid Timestamp, The timestamp has expired";
            return false;
        }
        return true;
    }

    protected bool VerifyNonce(string nonce, VerifyResult result)
    {
        if (_signatureValidatorOptions.IsValidateNonce)
        {
            if (_nonceRecorder.Record(nonce, _signatureValidatorOptions.TimestampValidationOffset))
            {
                return true;
            }
            result.ErrorMessage = "Invalid Nonce, Nonce has been used";
            return false;
        }
        return true;
    }

    protected bool VerifySignature(SignatureHeader signatureHeader, SignatureText signatureText, VerifyResult result)
    {
        var signatureGenerator = _signatureGeneratorRegistry.GetGenerator(signatureHeader.SignatureMethod);
        var signatureTextString = signatureText.ToString();
        var signatureString = signatureGenerator.Signature(_sKGetter.GetSK(signatureHeader.Key), signatureTextString);
        if (signatureString == signatureHeader.Signature)
        {
            return true;
        }
        result.ErrorMessage = $"Invalid Signature, Server StringToSign:`{signatureTextString.Replace('\n', '#')}`";
        return false;
    }
}
