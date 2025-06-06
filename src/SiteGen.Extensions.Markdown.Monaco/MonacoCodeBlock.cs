using Markdig.Parsers;
using Markdig.Syntax;

namespace SiteGen.Extensions.Markdown.Monaco;

public class MonacoCodeBlock(BlockParser parser) : FencedCodeBlock(parser)
{
}
