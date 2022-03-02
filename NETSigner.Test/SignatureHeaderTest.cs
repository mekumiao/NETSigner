using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NETSigner.Test;

[TestClass]
public class SignatureHeaderTest
{
    [TestMethod]
    public void TryParse()
    {
        var result = new VerifyResult();
        var headers = new Dictionary<string, string?>()
        {
            { SignatureConstant.Accept, "application/json; charset=utf-8" },
            { SignatureConstant.XCaKey, "123123" },
            { SignatureConstant.XCaTimestamp, "1646182350909" },
            { SignatureConstant.XCaNonce, "1002" },
            { SignatureConstant.ContentMD5, "eeeeeeeee" },
            { SignatureConstant.XCaSignature, "eeeeeeeee" },
        };
        if (SignatureHeader.TryParse(headers, result, out var signatureHeader))
        {
            Assert.AreEqual(signatureHeader.Key, "123123");
            Assert.AreEqual(signatureHeader.Nonce, "1002");
            Assert.AreEqual(signatureHeader.Timestamp, 1646182350909);
            Assert.IsTrue(result.IsSuccess);
        }
    }
}
