using Markdig;
using Markdig.Extensions.AutoLinks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SiteGen.Core.Configuration;
using SiteGen.Core.Extensions.Markdown.Mermaid;
using SiteGen.Core.Services;
using SiteGen.Core.Services.Generators;
using SiteGen.Core.Services.Processors;

namespace SiteGen.Core;

public static class ServiceCollectionExtensions
{
    public static void ConfigureSiteGen(this IServiceCollection services)
    {
        services.ConfigurePlaywright();

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
        services.AddTransient<IMarkdownExtension, MermaidMarkdownExtension>();
        
        services.TryAddSingleton( (provider) =>
        {
            var pipeline = new MarkdownPipelineBuilder()
                
                //.UseAdvancedExtensions()
                .UseAbbreviations()
                .UseAutoIdentifiers()
                .UseCitations()
                .UseCustomContainers()
                .UseDefinitionLists()
                .UseEmphasisExtras()
                .UseFigures()
                .UseFooters()
                .UseFootnotes()
                .UseGridTables()
                .UseMathematics()
                .UseMediaLinks()
                .UsePipeTables()
                .UseListExtras()
                .UseTaskLists()
                //.UseDiagrams() Crashes if there's no CodeBlockRenderer
                .UseAutoLinks()
                .UseGenericAttributes()

                .UseSmartyPants()
                .UseYamlFrontMatter()
                .UseAutoLinks(new AutoLinkOptions { UseHttpsForWWWLinks = true });

            //pipeline.UseColorCode(HtmlFormatterType.Style, ColorCode.Styling.StyleDictionary.DefaultLight);

            foreach(var extension in provider.GetServices<IMarkdownExtension>())
            {
                pipeline.Extensions.Add(extension);
            }

            //pipeline.Use(provider.GetRequiredService<MermaidMarkdownExtension>());
            //pipeline.Use(provider.GetRequiredService<MonacoCodeMarkdownExtension>());
            //pipeline.Use(provider.GetRequiredService<PygmentsMarkdownExtension>());

            return pipeline.Build();
        });

        return services;
    }
}