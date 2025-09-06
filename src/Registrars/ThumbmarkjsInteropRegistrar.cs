using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.Thumbmarkjs.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;

namespace Soenneker.Blazor.Thumbmarkjs.Registrars;

/// <summary>
/// A Blazor interop library for the javascript fingerprinting library, thumbmarkjs
/// </summary>
public static class ThumbmarkjsInteropRegistrar
{
    /// <summary>
    /// Adds <see cref="IThumbmarkjsInterop"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddThumbmarkjsInteropAsScoped(this IServiceCollection services)
    {
        services.AddResourceLoaderAsScoped().TryAddScoped<IThumbmarkjsInterop, ThumbmarkjsInterop>();

        return services;
    }
}
