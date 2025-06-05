using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace SiteGen.Core.Extensions.Markdown.Mermaid;

public class MermaidDiagramParser : FencedBlockParserBase<MermaidBlock>
{
    public MermaidDiagramParser()
    {
        OpeningCharacters = new[] { '`' };
        MinimumMatchCount = 3;
        MaximumMatchCount = 3;
        InfoPrefix = "mermaid";
        InfoParser = MermaidInfoParser;
    }

    public string? DefaultClass { get; private set; }

    protected override MermaidBlock CreateFencedBlock(BlockProcessor processor)
    {
        var block = new MermaidBlock(this);
        if (DefaultClass != null)
        {
            block.GetAttributes().AddClass(DefaultClass);
        }
        return block;
    }

    bool MermaidInfoParser(BlockProcessor state, ref StringSlice line, IFencedBlock fenced, char openingCharacter)
    {
        return line.MatchLowercase("mermaid");
    }
}