using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace SiteGen.Extensions.Markdown.Prism;

public class PrismCodeBlockParser : FencedBlockParserBase<PrismCodeBlock>
{
    public PrismCodeBlockParser()
    {
        OpeningCharacters = new[] { '`' };
        MinimumMatchCount = 3;
        MaximumMatchCount = 3;
        InfoPrefix = "language-";
        InfoParser = PrismCodeInfoParser;
        DefaultClass = "";
    }

    public string? DefaultClass { get; private set; }

    protected override PrismCodeBlock CreateFencedBlock(BlockProcessor processor)
    {
        var block = new PrismCodeBlock(this);
        if (DefaultClass != null)
        {
            block.GetAttributes().AddClass(DefaultClass);
        }
        return block;
    }

    private bool PrismCodeInfoParser(BlockProcessor state, ref StringSlice line, IFencedBlock fenced, char openingCharacter)
    {
        var matches = DefaultInfoParser(state, ref line, fenced, openingCharacter);
        if (!matches) return false;
        return !line.MatchLowercase("mermaid");
    }
}