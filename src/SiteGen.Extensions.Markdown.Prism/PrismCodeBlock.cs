using Markdig.Parsers;
using Markdig.Syntax;

namespace SiteGen.Extensions.Markdown.Prism;

public class PrismCodeBlock(BlockParser parser) : FencedCodeBlock(parser)
{
}
