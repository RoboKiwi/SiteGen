using SiteGen.Core.Configuration;
using SiteGen.Core.Extensions;
using SiteGen.Core.Models;

namespace SiteGen.Core.Services.Generators;

public class TaxonomyGenerator : INodeGenerator
{
    private readonly SiteGenSettings settings;

    public TaxonomyGenerator(SiteGenSettings settings)
    {
        this.settings = settings;
    }

    bool HasTaxonomyInFrontMatter(SiteNode node, string key, string? value)
    {
        if (node?.FrontMatter == null) return false;
        return (value != null && node.FrontMatter.ContainsKey(value)) || (key != null && node.FrontMatter.ContainsKey(key));
    }

    IList<string> GetTaxonomyValuesFromFrontMatter(SiteNode node, string key, string? value)
    {
        var values = new List<string>();

        var i = 0;
        while( i < 100)
        {
            if( value != null && node.FrontMatter.TryGetValue(value + ":" + i, out var result) && result != null)
            {
                values.Add(result);
            }
            else if( key != null && node.FrontMatter.TryGetValue(key + ":" + i, out var kValue) && kValue != null)
            {
                if(kValue != null) values.Add(kValue);
            }
            else
            {
                break;
            }
            i++;
        }

        return values;
    }

    public Task GenerateAsync(SiteMap nodes)
    {
        foreach(var (key, value) in settings.Taxonomies)
        {
            var uri = UrlBuilder.Build(value ?? key);

            // We might already have a node for this taxonomy if there is
            // a directory and/or index file for it (usually for setting front matter and content).
            var node = nodes.FindByUri(uri);

            // Create a section page for each taxonomy
            if (node == null)
            {
                node = new SiteNode
                {
                    Type = NodeType.Section,
                    Title = Humanizer.FromString(value ?? key),
                    Url = uri,
                    Path = "\\" + value ?? key
                };

                nodes.Add(node);
                nodes.Root.Tree.Children.Add(node);
            }

            // Discover all the values for this taxonomy found in all front matter
            var values = nodes.Where( x => HasTaxonomyInFrontMatter(x, key, value))
                .SelectMany(x => GetTaxonomyValuesFromFrontMatter(x, key, value))
                .Distinct()
                .ToList();

            foreach (var taxonomyValue in values)
            {
                var valueUri = UrlBuilder.Build(uri.ToString(), UrlBuilder.Friendly(taxonomyValue) );

                var taxonomyNode = nodes.FindByUri(valueUri) ?? new SiteNode
                {
                    Type = NodeType.Section,
                    Title = taxonomyValue,
                    Url = valueUri,
                    Path = node.Path + "\\" + taxonomyValue
                };

                // Get all nodes that are assigned to this category
                var matches = nodes.Where(x => HasTaxonomyInFrontMatter(x, key, value)
                    && GetTaxonomyValuesFromFrontMatter(x, key, value)
                        .Contains(taxonomyValue, StringComparer.OrdinalIgnoreCase)).ToList();

                foreach (var match in matches)
                {
                    // Only add a link; adding child will re-assign the parent to the category page!
                    taxonomyNode.Tree.Children.AddLink(match);
                }

                node.Tree.Children.Add(taxonomyNode);
                nodes.Add(taxonomyNode);
            }
        }

        return Task.CompletedTask;
    }
}
