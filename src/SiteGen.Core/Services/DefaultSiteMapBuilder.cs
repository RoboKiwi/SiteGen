using SiteGen.Core.Models.Hierarchy;
using SiteGen.Core.Services.Generators;
using SiteGen.Core.Services.Processors;

namespace SiteGen.Core.Services;

public class DefaultSiteMapBuilder : ISiteMapBuilder
{
    private MarkdownGenerator markdownGenerator;
    private readonly FrontMatterProcessor frontMatterProcessor;
    private readonly TaxonomyGenerator taxonomyGenerator;
    private readonly SiteMap map = new SiteMap();
    private bool isInitialized = false;
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

    public DefaultSiteMapBuilder(MarkdownGenerator generator, FrontMatterProcessor frontMatterProcessor, TaxonomyGenerator taxonomyGenerator)
    {
        markdownGenerator = generator;
        this.frontMatterProcessor = frontMatterProcessor;
        this.taxonomyGenerator = taxonomyGenerator;
    }

    public async Task<SiteMap> BuildAsync()
    {
        if (isInitialized) return map;
        await semaphore.WaitAsync();
        if (isInitialized) return map;

        try
        {
            await markdownGenerator.GenerateAsync(map);

            // Should be one root node
            var root = map.Single();

            // Parse the front matter for all the content files, and get the flat list of nodes while we're at it.
            foreach (var node in root.FlattenTree())
            {
                await frontMatterProcessor.ProcessAsync(node);
                map.Add(node);
            }

            // Build the tree hierarchy (parents / children)
            map.RebuildTree();

            // Now enhance with additional generator for taxonomy
            await taxonomyGenerator.GenerateAsync(map);

            // Rebuild again unfortunately
            map.RebuildTree();

            isInitialized = true;

            return map;
        }
        finally
        {
            semaphore.Release();
        }
    }
}