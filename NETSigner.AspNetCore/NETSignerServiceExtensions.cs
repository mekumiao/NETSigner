using Microsoft.Extensions.DependencyInjection.Extensions;
using NETSigner;

namespace Microsoft.Extensions.DependencyInjection;

public static class NETSignerServiceExtensions
{
    public static IServiceCollection AddNETSigner(this IServiceCollection services, Action<SignatureValidatorOptions> configurationOptions)
    {
        var options = new SignatureValidatorOptions();
        configurationOptions.Invoke(options);
        services.TryAddSingleton(options);
        services.TryAddSingleton<ISKGetter, DefaultSKGetter>();
        services.TryAddSingleton<INonceRecorder, DefaultNonceRecorder>();
        services.TryAddSingleton<SignatureTextGenerator>();
        services.TryAddSingleton<SignatureHeaderGenerator>();
        services.TryAddSingleton<SignatureGeneratorRegistry>();
        services.TryAddSingleton<SignatureValidator>();
        return services;
    }
}
