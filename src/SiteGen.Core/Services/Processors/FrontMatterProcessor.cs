using SiteGen.Core.Configuration.Yaml;
using SiteGen.Core.Models;
using System.Text;
using Tommy.Extensions.Configuration;

namespace SiteGen.Core.Services.Processors;

/// <summary>
/// Parses any front matter from the content, providing the values in
/// <see cref="SiteNode.FrontMatter"/> and also overriding the site node properties
/// with any specified in the front matter, such as <see cref="SiteNode.Url"/>.
/// </summary>
public class FrontMatterProcessor : ISiteNodeProcessor
{
    static readonly IList<string> skipBinding = new List<string> { "guid", "type" };

    public Task ProcessAsync(SiteNode node, CancellationToken cancellationToken)
    {
        // Load the front matter values
        var frontMatter = FrontMatterParser.ReadBlock(node.Content);

        if (frontMatter == null) return Task.CompletedTask;

        var builder = new ConfigurationBuilder();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(frontMatter.Item2));

        switch (frontMatter.Item1)
        {
            case FrontMatterFormat.None:
                break;
            case FrontMatterFormat.Yaml:
                builder.AddYamlStream(stream);
                break;
            case FrontMatterFormat.Json:
                builder.AddJsonStream(stream);
                break;
            case FrontMatterFormat.Toml:
                builder.AddTomlStream(stream);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(FrontMatterFormat), frontMatter.Item1, "Unsupported front matter format");
        }

        var root = builder.Build();
        var dictionary = root.AsEnumerable().ToDictionary(pair => pair.Key, pair => pair.Value);
        node.FrontMatter = dictionary.ToDictionary(pair => pair.Key, pair => pair.Value);

        foreach (var key in skipBinding)
        {
            if (!dictionary.ContainsKey(key)) continue;
            dictionary.Remove(key);
        }

        try
        {
            new ConfigurationBuilder()
                .AddInMemoryCollection(dictionary)
                .Build()
                .Bind(node);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error when loading front matter for {node.Path}");
            Console.WriteLine(ex);
        }

        return Task.CompletedTask;
    }
}