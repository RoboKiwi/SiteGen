using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace SiteGen.Core.Extensions.Markdown.Pygments;

public class PygmentsBlockParser : FencedBlockParserBase<PygmentsBlock>
{
    public PygmentsBlockParser()
    {
        OpeningCharacters = new[] { '`' };
        MinimumMatchCount = 3;
        MaximumMatchCount = 3;
        InfoPrefix = "language-";
        InfoParser = PygmentsInfoParser;
        DefaultClass = "chroma";
    }

    public string? DefaultClass { get; private set; }

    protected override PygmentsBlock CreateFencedBlock(BlockProcessor processor)
    {
        var block = new PygmentsBlock(this);
        if (DefaultClass != null)
        {
            block.GetAttributes().AddClass(DefaultClass);
        }
        return block;
    }

    bool PygmentsInfoParser(BlockProcessor state, ref StringSlice line, IFencedBlock fenced, char openingCharacter)
    {
        var matches = DefaultInfoParser(state, ref line, fenced, openingCharacter);
        if (!matches) return false;
        return !line.MatchLowercase("mermaid");
    }
}