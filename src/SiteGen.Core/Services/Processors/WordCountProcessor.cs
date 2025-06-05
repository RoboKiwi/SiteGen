using SiteGen.Core.Extensions;
using SiteGen.Core.Models;

namespace SiteGen.Core.Services.Processors;

public class WordCountProcessor : ISiteNodeProcessor
{
    public Task ProcessAsync(SiteNode node, CancellationToken cancellationToken)
    {
        node.WordCount = (node.ContentPlainText ?? "").AsSpan().WordCount();
        return Task.CompletedTask;
    }
}
