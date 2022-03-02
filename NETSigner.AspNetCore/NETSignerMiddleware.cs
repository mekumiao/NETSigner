using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NETSigner.AspNetCore;

public class NETSignerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<NETSignerMiddleware> _logger;
    private readonly SignatureValidator _signatureValidator;

    public NETSignerMiddleware(
        RequestDelegate next,
        ILoggerFactory loggerFactory,
        SignatureValidator signatureValidator)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<NETSignerMiddleware>();
        _signatureValidator = signatureValidator;
    }

    public async Task Invoke(HttpContext context)
    {
        var model = GetRequestModel(context.Request);
        var result = _signatureValidator.Verify(model);
        if (result.IsSuccess)
        {
            _logger.LogInformation("signer success");
            await _next(context);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(result.ErrorMessage);
        }
    }

    private static HttpRequestModel GetRequestModel(HttpRequest request)
    {
        var headers = request.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
        var model = new HttpRequestModel(headers)
        {
            Method = request.Method,
            Path = request.Path,
            Query = request.Query.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault())
        };
        if (request.Method == "POST" && request.HasFormContentType)
        {
            model.Form = request.Form.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
        }
        return model;
    }
}
