using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NETSigner.Test;

[TestClass]
public class SignatureTextGeneratorTest
{
    [TestMethod]
    public void TryGetSignatureText()
    {
        var result = new VerifyResult();
        var header = new Dictionary<string, string?>()
        {
            { SignatureConstant.Accept, "application/json; charset=utf-8" },
            { SignatureConstant.XCaKey, "123123" },
            { SignatureConstant.XCaTimestamp, "1646182350909" },
            { SignatureConstant.XCaNonce, "1002" },
            { SignatureConstant.ContentMD5, "eeeeeeeee" },
            { SignatureConstant.ContentType, "application/json; charset=utf-8" }
        };
        var model = new HttpRequestModel(header)
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
        };
        var textGenerator = new SignatureTextGenerator();
        if (textGenerator.TryGetSignatureText(model, result, out var text))
        {
            Assert.AreEqual(@"GET
application/json; charset=utf-8
eeeeeeeee
application/json; charset=utf-8

x-ca-key:123123
x-ca-nonce:1002
x-ca-timestamp:1646182350909
/sms/send?age=2&name=w&query=w&w=c
".Replace("\r", ""), text.ToString());
            Assert.IsTrue(text.IsSuccess);
        }
    }
}
