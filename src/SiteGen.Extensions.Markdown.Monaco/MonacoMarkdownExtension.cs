using Markdig;
using Markdig.Renderers;

namespace SiteGen.Extensions.Markdown.Monaco;

public class MonacoMarkdownExtension(MonacoHost host) : IMarkdownExtension
{
    readonly MonacoHost host = host;

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (pipeline.BlockParsers.Contains<MonacoCodeBlockParser>()) return;

        // High precedence so we can render before syntax highlighting
        pipeline.BlockParsers.Insert(0, new MonacoCodeBlockParser());
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not HtmlRenderer htmlRenderer) return;
        if (htmlRenderer.ObjectRenderers.Contains<MonacoCodeBlockRenderer>()) return;
        htmlRenderer.ObjectRenderers.Insert(0, new MonacoCodeBlockRenderer(host));
    }
}
