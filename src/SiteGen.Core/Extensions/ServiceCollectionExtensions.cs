using Markdig;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SiteGen.Core.Extensions.Markdown.Mermaid;
using SiteGen.Core.Extensions.Markdown.Pygments;
using SiteGen.Core.Models;
using SiteGen.Core.Services;
using SiteGen.Core.Services.Processors;

namespace SiteGen.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureSiteGen(this IServiceCollection services)
    {
        services.ConfigureMarkdig();

        services.ConfigureProcessors();

        services.TryAddSingleton<SiteMapBuilder>();
        services.TryAddSingleton<ISiteMapService, SiteMapService>();
        services.TryAddSingleton<SitePipelineBuilder>();
    }

    public static IServiceCollection ConfigureProcessors(this IServiceCollection services)
    {
        services.AddTransient<ISiteNodeProcessor, MarkdownProcessor>();
        services.AddTransient<ISiteNodeProcessor, WordCountProcessor>();
        services.AddTransient<ISiteNodeProcessor, WordCountFuzzyProcessor>();
        services.AddTransient<ISiteNodeProcessor, ReadingTimeProcessor>();
        services.AddTransient<ISiteNodeProcessor, TableOfContentsProcessor>();

        return services;
    }

    public static IServiceCollection ConfigureMarkdig(this IServiceCollection services)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseSmartyPants()
            .UseYamlFrontMatter()
            .Use<MermaidMarkdownExtension>()
            .Use<PygmentsMarkdownExtension>();

        services.TryAddSingleton(pipeline.Build());

        return services;
    }
}
