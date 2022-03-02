using NETSigner.AspNetCore;

namespace Microsoft.AspNetCore.Builder;

public static class NETSignerAppBuilderExtensions
{
    public static IApplicationBuilder UseNETSigner(this IApplicationBuilder app)
    {
        app.UseMiddleware<NETSignerMiddleware>();
        return app;
    }
}
