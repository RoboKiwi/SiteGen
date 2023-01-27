using Markdig;
using Markdig.Extensions.AutoLinks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SiteGen.Core.Configuration;
using SiteGen.Core.Extensions.Markdown.Mermaid;
using SiteGen.Core.Extensions.Markdown.Pygments;
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

        services.TryAddSingleton<ISiteMapBuilder, DefaultSiteMapBuilder>();
        services.TryAddSingleton<FileCacheProvider>();
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
        services.AddTransient<ISiteNodeProcessor, GitInfoProcessor>();
        services.AddTransient<ISiteNodeProcessor, WordCountProcessor>();
        services.AddTransient<ISiteNodeProcessor, WordCountFuzzyProcessor>();
        services.AddTransient<ISiteNodeProcessor, ReadingTimeProcessor>();
        services.AddTransient<ISiteNodeProcessor, TableOfContentsProcessor>();

        return services;
    }

    public static IServiceCollection ConfigureMarkdig(this IServiceCollection services)
    {
        services.AddTransient<MermaidMarkdownExtension>();
        services.AddTransient<PygmentsMarkdownExtension>();

        services.TryAddSingleton( (provider) =>
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSmartyPants()
                .UseYamlFrontMatter()
                .UseAutoLinks(new AutoLinkOptions { UseHttpsForWWWLinks = true });
            
            pipeline.Use(provider.GetRequiredService<MermaidMarkdownExtension>());
            pipeline.Use(provider.GetRequiredService<PygmentsMarkdownExtension>());
                        
            return pipeline.Build();
        });

        return services;
    }
}