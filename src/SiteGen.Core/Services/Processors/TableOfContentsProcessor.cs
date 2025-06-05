using SiteGen.Core.Extensions.Markdown;
using SiteGen.Core.Models;

namespace SiteGen.Core.Services.Processors;

public class TableOfContentsProcessor : ISiteNodeProcessor
{
    public Task ProcessAsync(SiteNode node, CancellationToken cancellationToken)
    {
        node.TableOfContents = node.Document?.ToTableOfContents();
        return Task.CompletedTask;
    }
}
