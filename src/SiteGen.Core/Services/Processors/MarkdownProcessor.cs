using Markdig;
using SiteGen.Core.Extensions;
using SiteGen.Core.Extensions.Markdown;
using SiteGen.Core.Models;

namespace SiteGen.Core.Services.Processors;

/// <summary>
/// Processes the document Markdown content, setting
/// the <see cref="SiteNode.ContentPlainText"/> and <see cref="SiteNode.HtmlContent"/>.
/// </summary>
public class MarkdownProcessor : ISiteNodeProcessor
{
    private readonly MarkdownPipeline pipeline;

    public MarkdownProcessor(MarkdownPipeline pipeline)
    {
        this.pipeline = pipeline;
    }

    public Task ProcessAsync(SiteNode node)
    {
        if (node.Content.IsEmpty()) return Task.CompletedTask;

        node.Document = Markdig.Markdown.Parse(node.Content, pipeline);

        node.HtmlContent = node.Document.ToHtml(pipeline);
        node.ContentPlainText = node.Document.ToPlainText(pipeline);

        return Task.CompletedTask;
    }
}
