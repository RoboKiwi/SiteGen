using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace SiteGen.Extensions.Markdown.Monaco;

public class MonacoCodeBlockParser : FencedBlockParserBase<MonacoCodeBlock>
{
    public MonacoCodeBlockParser()
    {
        OpeningCharacters = new[] { '`' };
        MinimumMatchCount = 3;
        MaximumMatchCount = 3;
        InfoPrefix = "language-";
        InfoParser = MonacoCodeInfoParser;
        DefaultClass = "";
    }

    public string? DefaultClass { get; private set; }

    protected override MonacoCodeBlock CreateFencedBlock(BlockProcessor processor)
    {
        var block = new MonacoCodeBlock(this);
        if (DefaultClass != null)
        {
            block.GetAttributes().AddClass(DefaultClass);
        }
        return block;
    }

    private bool MonacoCodeInfoParser(BlockProcessor state, ref StringSlice line, IFencedBlock fenced, char openingCharacter)
    {
        var matches = DefaultInfoParser(state, ref line, fenced, openingCharacter);
        if (!matches) return false;
        return !line.MatchLowercase("mermaid");
    }
}