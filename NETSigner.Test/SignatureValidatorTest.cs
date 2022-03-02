using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NETSigner.Test;

[TestClass]
public class SignatureValidatorTest
{
    [TestMethod]
    public async Task Verify()
    {
        var application = new WebApplicationFactory<Program>();
        var client = application.CreateClient();

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json)
        {
            CharSet = "charset=utf-8"
        });

        var content = new StringContent("{\"name\":\"kkk\"}", Encoding.UTF8, MediaTypeNames.Application.Json);
        content.Headers.ContentType = new(MediaTypeNames.Application.Json) { CharSet = "charset=utf-8" };
        content.Headers.ContentMD5 = await MD5.Create().ComputeHashAsync(await content.ReadAsStreamAsync());
        content.Headers.Add(SignatureConstant.XCaKey, "123123");
        content.Headers.Add(SignatureConstant.XCaTimestamp, "123123");
        content.Headers.Add(SignatureConstant.XCaNonce, "10000");
        content.Headers.Add(SignatureConstant.XCaSignatureMethod, "HmacSHA256");
        content.Headers.Add(SignatureConstant.XCaSignature, "x");

        var resp = await client.PostAsync("/sign", content);
        Assert.IsTrue(resp.StatusCode == HttpStatusCode.Unauthorized);
    }
}
