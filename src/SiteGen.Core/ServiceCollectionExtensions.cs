using Markdig;
using Markdig.Extensions.AutoLinks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SiteGen.Core.Configuration;
using SiteGen.Core.Extensions.Markdown.Mermaid;
using SiteGen.Core.Extensions.Markdown.Pygments;
using SiteGen.Core.Models;
using SiteGen.Core.Services;
using SiteGen.Core.Services.Generators;
using SiteGen.Core.Services.Processors;

namespace SiteGen.Core;

public static class ServiceCollectionExtensions
{
    public static void ConfigureSiteGen(this IServiceCollection services)
    {
        services.ConfigureMarkdig();

        services.ConfigureProcessors();

        services.ConfigureGenerators();

        services.AddOptions<SiteGenSettings>();

        //services.TryAddSingleton<ISiteMapService, SiteMapService>();
        services.TryAddSingleton<ISiteMapBuilder, DefaultSiteMapBuilder>();
    }

    public static IServiceCollection ConfigureGenerators(this IServiceCollection services)
    {
        services.AddTransient<MarkdownGenerator>();
        services.AddTransient<TaxonomyGenerator>();
        return services;
    }

    public static IServiceCollection ConfigureProcessors(this IServiceCollection services)
    {
        services.AddTransient<FrontMatterProcessor>();
        services.AddTransient<MarkdownProcessor>();
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
            .UseAutoLinks(new AutoLinkOptions { UseHttpsForWWWLinks = true })
            .Use<MermaidMarkdownExtension>()
            .Use<PygmentsMarkdownExtension>()
            ;

        services.TryAddSingleton(pipeline.Build());

        return services;
    }
}