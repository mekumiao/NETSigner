using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NETSigner.Test;

[TestClass]
public class SignatureValidatorTest
{
    protected static WebApplicationFactory<Program> GetWebApplication() => new WebApplicationFactory<Program>();
    protected static HttpClient CreateClient() => GetWebApplication().CreateDefaultClient(new SignatureDelegatingHandler());

    [TestMethod]
    public async Task VerifyUnauthorized()
    {
        var client = CreateClient();
        var content = new StringContent("{\"name\":\"kkk\"}", Encoding.UTF8, MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add(SignatureConstant.XCaSignature, "x");
        var resp = await client.PostAsync("/sign", content);
        Assert.IsTrue(resp.StatusCode == HttpStatusCode.Unauthorized);
    }

    [TestMethod]
    public async Task VerifySuccess()
    {
        var client = CreateClient();
        var content = new StringContent("{\"name\":\"kkk\"}", Encoding.UTF8, MediaTypeNames.Application.Json);
        var resp = await client.PostAsync("/sign", content);
        var message = await resp.Content.ReadAsStringAsync();
        Assert.IsTrue(resp.IsSuccessStatusCode, message);
    }
}

public class SignatureDelegatingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Content?.Headers.ContentType != null)
        {
            if (request.Content.Headers.ContentType.MediaType != "application/x-www-form-urlencoded")
            {
                using var contentStream = await request.Content.ReadAsStreamAsync();
                request.Content.Headers.ContentMD5 = await MD5.Create().ComputeHashAsync(contentStream);
            }
        }

        request.Headers.Accept.Clear();
        request.Headers.AcceptCharset.Clear();

        request.Headers.Date = DateTimeOffset.Now;
        request.Headers.Accept.Add(new(MediaTypeNames.Application.Json));
        request.Headers.AcceptCharset.Add(new(Encoding.UTF8.HeaderName));

        request.Headers.Add(SignatureConstant.XCaKey, "123123");
        request.Headers.Add(SignatureConstant.XCaTimestamp, DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
        request.Headers.Add(SignatureConstant.XCaNonce, Guid.NewGuid().ToString());
        request.Headers.Add(SignatureConstant.XCaSignatureMethod, "HmacSHA256");

        var result = new VerifyResult();
        var headers = request.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
        if (request.Content?.Headers.ContentType != null)
        {
            headers.TryAdd(SignatureConstant.ContentType, request.Content.Headers.ContentType.ToString());
            headers.TryAdd(SignatureConstant.ContentMD5, Convert.ToBase64String(request.Content.Headers.ContentMD5!));
        }
        if (SignaturePlaintext.TryParse(request.RequestUri!.LocalPath, request.Method.ToString(), headers, result, out var plaintext))
        {
            var hmacSHA256 = new HmacSHA256SignatureGenerator();
            var plaintextString = plaintext.ToString();
            var sign = hmacSHA256.Signature("123123", plaintextString);
            request.Headers.Add(SignatureConstant.XCaSignature, sign);
        }
        else
        {
            throw new InvalidOperationException(result.ErrorMessage);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
