using Markdig.Parsers;
using Markdig.Syntax;

namespace SiteGen.Core.Extensions.Markdown.Pygments;

public class PygmentsBlock : FencedCodeBlock
{
    public PygmentsBlock(BlockParser parser) : base(parser) { }
}
