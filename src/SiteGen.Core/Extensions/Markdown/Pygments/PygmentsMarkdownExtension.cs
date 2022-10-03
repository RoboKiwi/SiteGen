using Markdig;
using Markdig.Renderers;

namespace SiteGen.Core.Extensions.Markdown.Pygments;

public class PygmentsMarkdownExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (pipeline.BlockParsers.Contains<PygmentsBlockParser>()) return;

        // High precendence so we can render before syntax highlighting
        pipeline.BlockParsers.Insert(0, new PygmentsBlockParser());
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not HtmlRenderer htmlRenderer) return;
        if (htmlRenderer.ObjectRenderers.Contains<PygmentsBlockRenderer>()) return;
        htmlRenderer.ObjectRenderers.Insert(0, new PygmentsBlockRenderer());
    }
}
