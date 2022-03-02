using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NETSigner.Test;

[TestClass]
public class SignatureTextTest
{
    private IDictionary<string, string?> _headers = new Dictionary<string, string?>()
    {
        { SignatureConstant.XCaKey, "123123" },
        { SignatureConstant.XCaTimestamp, "1646182350909" },
        { SignatureConstant.XCaNonce, "10000" },
        { SignatureConstant.Accept, "application/json; charset=utf-8" },
        { SignatureConstant.ContentType, "application/x-www-form-urlencoded; charset=utf-8" },
        { SignatureConstant.Date, "Wed, 02 Mar 2022 09:36:55 GMT+08:00"},
    };
    private IDictionary<string, string?> _querys = new Dictionary<string, string?>()
    {
        { "query", "w" },
    };
    private IDictionary<string, string?> _forms = new Dictionary<string, string?>()
    {
        { "name", "xiaowang" },
        { "age", "12" },
        { "sex", "0" },
        { "qq", null },
    };

    [DataTestMethod]
    [DataRow("/sign", "POST")]
    [DataRow("/signtt", "GET")]
    public void TryParse(string path, string method)
    {
        var result = new VerifyResult();
        if (SignatureText.TryParse(path: path, method: method, headers: _headers, result: result, signatureText: out var signatureText))
        {
            Assert.IsTrue(result.IsSuccess);
            signatureText.Querys = _querys;
            signatureText.Forms = _forms;
            var signature =
    @$"{method}
application/json; charset=utf-8
application/x-www-form-urlencoded; charset=utf-8
Wed, 02 Mar 2022 09:36:55 GMT+08:00
x-ca-key:123123
x-ca-nonce:10000
x-ca-timestamp:1646182350909
{path}?age=12&name=xiaowang&qq&query=w&sex=0
";
            Assert.AreEqual(signature.Replace("\r", ""), signatureText.ToString());
        }
    }
}
