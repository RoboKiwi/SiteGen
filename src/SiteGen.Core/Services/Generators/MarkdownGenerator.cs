using SiteGen.Core.Configuration;
using SiteGen.Core.Extensions;
using SiteGen.Core.Models;
using SiteGen.Core.Models.Hierarchy;
using System.Text;

namespace SiteGen.Core.Services.Generators;

public class MarkdownGenerator : INodeGenerator
{
    private readonly SiteGenSettings settings;
    private readonly ILogger<MarkdownGenerator> logger;

    public MarkdownGenerator(SiteGenSettings settings, ILogger<MarkdownGenerator> logger)
    {
        this.settings = settings;
        this.logger = logger;
    }

    public async Task GenerateAsync(SiteMap nodes)
    {
        foreach(var path in settings.ContentPaths)
        {
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
            {
                logger.LogWarning("Content path directory doesn't exist: {FullPath} ({Path})", directory.FullName, path);
                continue;
            }

            var node = await GenerateFromDirectoryAsync(directory, directory);
            if( node != null)
            {
                nodes.Add(node);
            }
        }
    }

    async Task<DirectorySiteNode?> GenerateFromDirectoryAsync(DirectoryInfo directory, DirectoryInfo baseDirectory)
    {
        if (!directory.Exists)
        {
            logger.LogWarning("Content path directory doesn't exist: {FullPath}", directory.FullName);
            return null;
        }

        var node = new DirectorySiteNode(directory, baseDirectory);

        node.Tree = new TreeInfo<SiteNode>(node);

        // Process sub directories first
        foreach (var subDirectory in directory.GetDirectories())
        {
            var child = await GenerateFromDirectoryAsync(subDirectory, baseDirectory);
            if (child == null) continue;
            node.Tree.Children.Add(child);
        }

        // Process files
        foreach (var file in directory.GetFiles("*.md"))
        {
            var isIndex = file.Name.Equals("_index.md", StringComparison.OrdinalIgnoreCase);

            if(isIndex)
            {
                node.Content = await File.ReadAllTextAsync(file.FullName, Encoding.UTF8);
            }
            else
            {
                var child = await BuildNodeAsync(file, baseDirectory);
                if (child == null) continue;
                node.Tree.Children.Add(child);
            }
        }

        return node;
    }

    async Task<SiteNode?> BuildNodeAsync(FileInfo file, DirectoryInfo baseDirectory)
    {
        var filename = file.FullName;

        var node = new SiteNode
        {
            Path = filename,
            FileName = file.Name,
            Ext = file.Extension,
            Type = NodeType.Page,
            Title = Humanizer.FromFilename(file)
        };

        if (file.Exists)
        {
            node.Date = file.LastWriteTime < file.CreationTime ? file.LastWriteTime : file.CreationTime;
            node.DateCreated = file.CreationTime;
            node.DateModified = file.LastWriteTime;
        }

        node.Url = UrlBuilder.RelativeToDirectory(file, baseDirectory);

        node.Tree = new TreeInfo<SiteNode>(node);

        node.Content = await File.ReadAllTextAsync(filename, Encoding.UTF8, CancellationToken.None);

        return node;
    }
}
