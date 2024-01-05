using Markdig;
using Markdig.Extensions.AutoLinks;
using Microsoft.AspNetCore.NodeServices;
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

        services.ConfigureNodeServices();

        services.AddOptions<SiteGenSettings>();

        services.TryAddSingleton<ISiteMapBuilder, DefaultSiteMapBuilder>();
        services.TryAddSingleton<FileCacheProvider>();
    }

    public static IServiceCollection ConfigureNodeServices(this IServiceCollection services) => services.ConfigureNodeServices((_) => { });

    public static IServiceCollection ConfigureNodeServices(this IServiceCollection services, Action<NodeServicesOptions> setupAction)
    {
        services.AddSingleton(typeof(INodeServices), serviceProvider =>
        {
            var fileCache = serviceProvider.GetRequiredService<FileCacheProvider>();
            
            var options = new NodeServicesOptions(serviceProvider);

            options.ProjectPath = fileCache.Directory.FullName;// Path.Combine(AppContext.BaseDirectory, "node_modules")

            var path = Environment.GetEnvironmentVariable("PATH");

            var nodePath = Path.Combine(Path.GetTempPath(), "SiteGen", "node");

            Environment.SetEnvironmentVariable("PATH", nodePath + Path.PathSeparator + (Environment.GetEnvironmentVariable("PATH") ?? ""));
            
            setupAction(options);
            
            return NodeServicesFactory.CreateNodeServices(options);
        });

        return services;
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