using Markdig;
using Markdig.Renderers;
using SiteGen.Core.Services;

namespace SiteGen.Core.Extensions.Markdown.Mermaid;

public class MermaidMarkdownExtension : IMarkdownExtension
{
    private readonly FileCacheProvider cache;

    public MermaidMarkdownExtension(FileCacheProvider cache)
    {
        this.cache = cache;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (pipeline.BlockParsers.Contains<MermaidDiagramParser>()) return;

        // High precedence so we can render before syntax highlighting
        pipeline.BlockParsers.Insert(0, new MermaidDiagramParser());
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not HtmlRenderer htmlRenderer) return;
        if (htmlRenderer.ObjectRenderers.Contains<MermaidBlockRenderer>()) return;
        htmlRenderer.ObjectRenderers.Insert(0, new MermaidBlockRenderer(cache));
    }
}
