using System.Text;
using SiteGen.Core.Configuration.Yaml;
using SiteGen.Core.Models.Hierarchy;
using SiteGen.Core.Services;
using SiteGen.Core.Services.Generators;
using Tommy.Extensions.Configuration;

namespace SiteGen.Core.Models;

public class SiteMapBuilder : ISiteMapBuilder
{
    private List<INodeGenerator> generators;

    public SiteMapBuilder(IServiceProvider services)
    {
        generators = services.GetServices<INodeGenerator>().ToList();
    }

    public async Task<SiteMap> BuildAsync()
    {
        var sitemap = new SiteMap();

        foreach(var generator in generators)
        {
            await generator.GenerateAsync(sitemap);
        }

        return sitemap;
    }

    public async Task<IList<SiteNode>> Build(string basePath, CancellationToken cancellationToken = default)
    {
        var directory = new DirectoryInfo(basePath);
        
        var node = await BuildAsync(directory, cancellationToken);

        if (node == null) throw new Exception("Failed to parse the content directory");

        var nodes = node.FlattenTree().ToList();

        // Category node

        // Build category nodes
        var categories = nodes.Where(x => x.Categories != null && x.Categories.Count > 0)
            .SelectMany(x => x.Categories)
            .Distinct()
            .ToList();

        var categoriesDirectory = new DirectoryInfo(Path.GetFullPath(Path.Combine(basePath, "categories")));

        var categoryNode = await BuildAsync(categoriesDirectory, false, false, true, cancellationToken);

        foreach(var cat in categories)
        {
            var categoryDirectory = new DirectoryInfo(Path.GetFullPath(Path.Combine(basePath, "categories", cat)));
            var category = await BuildAsync(categoryDirectory, false, false, true, cancellationToken);

            // Get all nodes that are assigned to this category
            var matches = nodes.Where(x => x.Categories != null && x.Categories.Contains(cat, StringComparer.OrdinalIgnoreCase)).ToList();

            foreach(var match in matches)
            {
                // Only add a link; adding child will re-assign the parent to the category page!
                category.Tree.Children.AddLink(match);
            }

            categoryNode.Tree.Children.Add(category);
            nodes.Add(category);
        }

        nodes.Add(categoryNode);
        node.Tree.Children.Add(categoryNode);
        
        // Update URIs
        foreach (var child in nodes)
        {
            if (child?.Url != null) continue;
                        
            var relativeFilename = Path.GetRelativePath(directory.FullName, child.Path);

            if (relativeFilename.EndsWith(".md"))
            {
                relativeFilename = relativeFilename.Substring(0, relativeFilename.Length - ".md".Length);
            }

            if (relativeFilename.EndsWith("_index"))
            {
                relativeFilename = relativeFilename.Substring(0, relativeFilename.Length - "_index".Length);
            }

            relativeFilename = "/" + relativeFilename.TrimStart('/', '\\').TrimEnd('/','\\').Replace('\\', '/') + "/";

            child.Url = new Uri(relativeFilename, UriKind.Relative);
        }

        nodes.RebuildTree();

        return nodes.ToList();
    }

    private Task<SiteNode> BuildAsync(DirectoryInfo directory, CancellationToken cancellationToken)
    {
        return BuildAsync(directory, true, true, true, cancellationToken);
    }

    private async Task<SiteNode> BuildAsync(DirectoryInfo directory, bool processDirectories, bool processChildren, bool processMetadata, CancellationToken cancellationToken)
    {
        var node = new SiteNode
        {
            FileName = directory.Name,
            Path = directory.FullName,
            Type = NodeType.Section,
            Title = directory.Name
        };

        node.Tree = new TreeInfo<SiteNode>(node);

        if (processDirectories && directory.Exists)
        {
            foreach (var subDirectory in directory.GetDirectories())
            {
                var child = await BuildAsync(subDirectory, cancellationToken);
                if (child == null) continue;
                node.Tree.Children.Add(child);
            }
        }

        if (directory.Exists && (processChildren || processMetadata))
        {
            foreach (var file in directory.GetFiles("*.md"))
            {
                var isIndex = file.Name.Equals("_index.md", StringComparison.OrdinalIgnoreCase);

                if (!processChildren && !isIndex) continue;

                var child = await BuildNodeAsync(file);

                if (child == null) continue;

                if (processMetadata && isIndex)
                {
                    // This is the section metadata file, so we can apply it to the parent node
                    node.FrontMatter = child.FrontMatter;
                    node.Content = child.Content;
                    node.Guid = child.Guid;
                    node.HtmlContent = child.HtmlContent;
                    node.Id = child.Id;
                    node.Author = child.Author;
                    node.Description = child.Description;
                    node.Url = child.Url;
                    node.Draft = child.Draft;
                    node.Summary = child.Summary;
                    node.Aliases = child.Aliases;
                    node.Tags = child.Tags;
                    node.WordCount = child.WordCount;
                    node.Categories = child.Categories;
                    node.Content = child.Content;
                    node.ContentPlainText = child.ContentPlainText;
                    node.Data = child.Data;
                    node.DateCreated = child.DateCreated;
                    node.DateExpired = child.DateExpired;
                    node.DateModified = child.DateModified;
                    node.DatePublished = child.DatePublished;
                    node.HtmlContent = child.HtmlContent;
                    node.Keywords = child.Keywords;
                    node.Lang = child.Lang;
                    node.LinkTitle = child.LinkTitle;
                    node.Permalink = child.Permalink;
                    node.ReadingTime = child.ReadingTime;
                    node.Title = child.Title;
                    node.Weight = child.Weight;
                    node.WordCount = child.WordCount;
                    node.WordCountFuzzy = child.WordCountFuzzy;
                    
                    continue;
                }

                node.Tree.Children.Add(child);
            }
        }

        return node;
    }

    private async Task<SiteNode?> BuildNodeAsync(FileInfo file)
    {
        var filename = file.FullName;

        var node = new SiteNode
        {
            Path = filename,
            FileName = Path.GetFileName(filename),
            Ext = Path.GetExtension(filename),
            Type = NodeType.Page
        };

        node.Tree = new TreeInfo<SiteNode>(node);

        node.Content = await File.ReadAllTextAsync(filename, Encoding.UTF8, CancellationToken.None);

        // Load the front matter values
        var frontMatter = FrontMatterParser.ReadBlock(node.Content);

        if (frontMatter != null)
        {
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
                    throw new ArgumentOutOfRangeException();
            }

            var root = builder.Build();
            var dictionary = root.AsEnumerable().ToDictionary(pair => pair.Key, pair => pair.Value);
            node.FrontMatter = dictionary.ToDictionary(pair => pair.Key, pair => pair.Value);

            //foreach(var key in skipBinding)
            //{
            //    if (!dictionary.ContainsKey(key)) continue;
            //    dictionary.Remove(key);
            //}
            
            try
            {
                var bindingRoot = new ConfigurationBuilder().AddInMemoryCollection(dictionary).Build();
                bindingRoot.Bind(node);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when loading front matter for {filename}");
                Console.WriteLine(ex);
                return null;
            }
        }

        return node;
    }
}
