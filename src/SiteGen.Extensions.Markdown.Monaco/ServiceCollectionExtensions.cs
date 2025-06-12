using Markdig;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SiteGen.Extensions.Markdown.Monaco;

namespace SiteGen.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseMonaco(this IServiceCollection services)
    {
        services.AddOptions<MonacoOptions>().Configure(options => { });

        services.TryAddTransient<IMarkdownExtension, MonacoMarkdownExtension>();
        
        return services;
    }

    public static IServiceCollection AddMonaco(this IServiceCollection services)
    {
        services.TryAddTransient<IMonacoService, MonacoService>();
        services.TryAddSingleton<MonacoHost>();
        return services;
    }
}

internal class MonacoOptions
{
    
}

public interface IMonacoService
{
    /// <summary>
    /// Returns the CSS for the specified theme.
    /// </summary>
    /// <param name="theme"></param>
    /// <returns></returns>
    Task<string> GetCssAsync(string theme);
}

public class MonacoService(MonacoHost host) : IMonacoService
{
    private readonly MonacoHost host = host;

    public Task<string> GetCssAsync(string theme)
    {
        return host.GetCssAsync(theme);
    }
}