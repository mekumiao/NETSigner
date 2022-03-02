using System.Security.Cryptography;
using NETSigner;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISKGetter, DefaultSKGetter>();
builder.Services.AddSingleton<INonceRecorder, DefaultNonceRecorder>();
builder.Services.AddSingleton<SignatureTextGenerator>();
builder.Services.AddSingleton<SignatureHeaderGenerator>();
builder.Services.AddSingleton<SignatureGeneratorRegistry>();
builder.Services.AddSingleton(provider => new SignatureValidatorOptions { TimestampValidationOffset = TimeSpan.FromDays(5) });
builder.Services.AddSingleton<SignatureValidator>();

var app = builder.Build();

app.Map("sign", async context =>
{
    var model = new HttpRequestModel
    {
        Path = context.Request.Path,
        Method = context.Request.Method,
    };
    model.Headers = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
    model.Query = context.Request.Query.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
    model.Headers.TryAdd(SignatureConstant.Accept, context.Request.Headers.Accept);
    if (context.Request.Method == "POST")
    {
        if (context.Request.HasFormContentType)
        {
            model.Form = context.Request.Form.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
        }
        else
        {
            model.ContentMD5 = Convert.ToBase64String(await MD5.Create().ComputeHashAsync(context.Request.Body));
        }
    }

    var validator = context.RequestServices.GetRequiredService<SignatureValidator>();
    var result = validator.Verify(model);
    if (result.IsSuccess)
    {
        await context.Response.WriteAsync("> success");
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync(result.ErrorMessage);
    }
});

await app.RunAsync();

public partial class Program { }
