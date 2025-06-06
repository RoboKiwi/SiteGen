using Markdig;
using Microsoft.Extensions.DependencyInjection;
using SiteGen.Extensions.Markdown.Monaco;

namespace SiteGen.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureMonaco(this IServiceCollection services)
    {
        services.AddTransient<IMarkdownExtension, MonacoMarkdownExtension>();
        services.AddSingleton<MonacoHost>();
        return services;
    }
}