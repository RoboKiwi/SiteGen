using SiteGen.Core.Models.Hierarchy;
using SiteGen.Core.Services.Generators;
using SiteGen.Core.Services.Processors;

namespace SiteGen.Core.Services;

public class DefaultSiteMapBuilder : ISiteMapBuilder
{
    MarkdownGenerator markdownGenerator;
    readonly FrontMatterProcessor frontMatterProcessor;
    readonly TaxonomyGenerator taxonomyGenerator;
    readonly SiteMap map = new();
    bool isInitialized;
    readonly SemaphoreSlim semaphore = new(1);

    public DefaultSiteMapBuilder(MarkdownGenerator generator, FrontMatterProcessor frontMatterProcessor, TaxonomyGenerator taxonomyGenerator)
    {
        markdownGenerator = generator;
        this.frontMatterProcessor = frontMatterProcessor;
        this.taxonomyGenerator = taxonomyGenerator;
    }

    public async Task<SiteMap> BuildAsync(CancellationToken cancellationToken)
    {
        if (isInitialized) return map;
        await semaphore.WaitAsync(cancellationToken);
        if (isInitialized) return map;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await markdownGenerator.GenerateAsync(map, cancellationToken);

            // Should be one root node
            var root = map.Single();

            // Parse the front matter for all the content files, and get the flat list of nodes while we're at it.
            foreach (var node in root.FlattenTree())
            {
                await frontMatterProcessor.ProcessAsync(node, cancellationToken);
                map.Add(node);
            }

            // Build the tree hierarchy (parents / children)
            map.RebuildTree();

            // Now enhance with additional generator for taxonomy
            await taxonomyGenerator.GenerateAsync(map, cancellationToken);

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