using Markdig.Parsers;
using Markdig.Syntax;

namespace SiteGen.Core.Extensions.Markdown.Mermaid;

public class MermaidBlock : FencedCodeBlock
{
    public MermaidBlock(BlockParser parser) : base(parser) { }
}
