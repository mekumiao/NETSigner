using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NETSigner.Test;

[TestClass]
public class SignatureTextTest
{
    [TestMethod]
    public void VerifyPOST()
    {
        var signatureText = new SignatureText
        {
            HTTPMethod = "POST",
            Accept = "application/json; charset=utf-8",
            ContentType = "application/x-www-form-urlencoded; charset=utf-8",
            Date = "Wed, 02 Mar 2022 09:36:55 GMT+08:00",
            Path = "/sign",
            Headers = new Dictionary<string, string?>()
            {
                { SignatureConstant.XCaKey, "123123" },
                { SignatureConstant.XCaTimestamp, "1646182350909" },
                { SignatureConstant.XCaNonce, "10000" },
            },
            Form = new Dictionary<string, string?>()
            {
                { "name", "xiaowang" },
                { "age", "12" },
                { "sex", "0" },
                { "qq", null },
            },
            Query = new Dictionary<string, string?>()
            {
                { "query", "w" },
            },
        };

        var signature =
@"POST
application/json; charset=utf-8
application/x-www-form-urlencoded; charset=utf-8
Wed, 02 Mar 2022 09:36:55 GMT+08:00
x-ca-key:123123
x-ca-nonce:10000
x-ca-timestamp:1646182350909
/sign?age=12&name=xiaowang&qq&query=w&sex=0
";
        Assert.AreEqual(signature.Replace("\r", ""), signatureText.ToString());
    }

    [TestMethod]
    public void VerifyGET()
    {
        var signatureText = new SignatureText
        {
            HTTPMethod = "GET",
            Accept = "application/json; charset=utf-8",
            ContentType = "application/x-www-form-urlencoded; charset=utf-8",
            Date = "Wed, 02 Mar 2022 09:36:55 GMT+08:00",
            Path = "/sign",
            Headers = new Dictionary<string, string?>()
            {
                { SignatureConstant.XCaKey, "123123" },
                { SignatureConstant.XCaTimestamp, "1646182350909" },
                { SignatureConstant.XCaNonce, "10000" },
            },
            Query = new Dictionary<string, string?>()
            {
                { "query", "w" },
            },
        };

        var signature =
@"GET
application/json; charset=utf-8
application/x-www-form-urlencoded; charset=utf-8
Wed, 02 Mar 2022 09:36:55 GMT+08:00
x-ca-key:123123
x-ca-nonce:10000
x-ca-timestamp:1646182350909
/sign?query=w
";
        Assert.AreEqual(signature.Replace("\r", ""), signatureText.ToString());
    }
}
