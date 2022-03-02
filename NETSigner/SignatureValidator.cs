namespace NETSigner;

/// <summary>
/// 签名验证器
/// </summary>
public class SignatureValidator
{
    private readonly ISKGetter _sKGetter;
    private readonly INonceRecorder _nonceRecorder;
    private readonly SignatureGeneratorRegistry _signatureGeneratorRegistry;
    private readonly SignatureValidatorOptions _signatureValidatorOptions;

    public SignatureValidator(
        ISKGetter sKGetter,
        INonceRecorder nonceRecorder,
        SignatureGeneratorRegistry signatureGeneratorRegistry,
        SignatureValidatorOptions signatureValidatorOptions)
    {
        _sKGetter = sKGetter;
        _nonceRecorder = nonceRecorder;
        _signatureGeneratorRegistry = signatureGeneratorRegistry;
        _signatureValidatorOptions = signatureValidatorOptions;
    }

    public VerifyResult Verify(HttpRequestModel requestModel)
    {
        var result = new VerifyResult();
        if (requestModel.Headers.TryGetValue(SignatureConstant.XCaSignature, out var signature) && !string.IsNullOrWhiteSpace(signature))
        {
            if (SignaturePlaintext.TryParse(requestModel.Path, requestModel.Method, requestModel.Headers, result, out var plaintext))
            {
                _ = VerifyTimestamp(plaintext.Timestamp, result) && VerifyNonce(plaintext.Nonce, result) && VerifySignature(signature, plaintext, result);
            }
        }
        else
        {
            result.ErrorMessage = $"Missing parameter, The {SignatureConstant.XCaSignature} is required";
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

    protected bool VerifySignature(string signature, SignaturePlaintext plaintext, VerifyResult result)
    {
        var signatureGenerator = _signatureGeneratorRegistry.GetGenerator(plaintext.SignatureMethod);
        var signatureTextString = plaintext.ToString();
        var signatureString = signatureGenerator.Signature(_sKGetter.GetSK(plaintext.Key), signatureTextString);
        if (signatureString == signature)
        {
            return true;
        }
        result.ErrorMessage = $"Invalid Signature, Server StringToSign:`{signatureTextString.Replace('\n', '#')}`";
        return false;
    }
}
