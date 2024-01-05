using Markdig;
using Markdig.Renderers;

namespace SiteGen.Extensions.Markdown.Prism;

public class PrismMarkdownExtension(PrismHost host) : IMarkdownExtension
{
    readonly PrismHost host = host;

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (pipeline.BlockParsers.Contains<PrismCodeBlockParser>()) return;

        // High precedence so we can render before syntax highlighting
        pipeline.BlockParsers.Insert(0, new PrismCodeBlockParser());
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not HtmlRenderer htmlRenderer) return;
        if (htmlRenderer.ObjectRenderers.Contains<PrismCodeBlockRenderer>()) return;
        htmlRenderer.ObjectRenderers.Insert(0, new PrismCodeBlockRenderer(host));
    }
}
