using Markdig;
using Microsoft.Extensions.DependencyInjection;
using SiteGen.Extensions.Markdown.Prism;

namespace SiteGen.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigurePrism(this IServiceCollection services)
    {
        services.AddTransient<IMarkdownExtension, PrismMarkdownExtension>();
        services.AddSingleton<PrismHost>();
        return services;
    }
}