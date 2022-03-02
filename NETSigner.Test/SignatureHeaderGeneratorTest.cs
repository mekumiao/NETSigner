using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NETSigner.Test;

[TestClass]
public class SignatureHeaderGeneratorTest
{
    [TestMethod]
    public void TryGetSignatureHeader()
    {
        var result = new VerifyResult();
        var model = new HttpRequestModel
        {
            Method = "GET",
            Path = "/sms/send",
            Query = new Dictionary<string, string?>()
            {
                { "w", "c" },
                { "query", "w" },
                { "age", "2" },
                { "name", "w" },
            },
            Headers = new Dictionary<string, string?>()
            {
                { SignatureConstant.Accept, "application/json; charset=utf-8" },
                { SignatureConstant.XCaKey, "123123" },
                { SignatureConstant.XCaTimestamp, "1646182350909" },
                { SignatureConstant.XCaNonce, "1002" },
                { SignatureConstant.ContentMD5, "eeeeeeeee" },
                { SignatureConstant.XCaSignature, "eeeeeeeee" },
            }
        };
        var headerGenerator = new SignatureHeaderGenerator();
        if (headerGenerator.TryGetSignatureHeader(model, result, out var text))
        {
            Assert.IsTrue(text.IsSuccess);
        }
    }
}
