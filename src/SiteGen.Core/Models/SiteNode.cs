using Markdig.Syntax;
using SiteGen.Core.Extensions.Markdown;
using SiteGen.Core.Models.Hierarchy;

namespace SiteGen.Core.Models;

public class SiteNode : IEntity, ITreeEntity<SiteNode>
{
    public SiteNode()
    {
        Tree = new TreeInfo<SiteNode>(this);
    }

    public Guid Guid { get; set; }

    public NodeType Type { get; set; }

    public string Path { get; set; }

    public string FileName { get; set; }

    public string Ext { get; set; }

    public string Content { get; set; }

    public IDictionary<string, string> FrontMatter { get; set; }

    public TreeInfo<SiteNode> Tree { get; set; }

    public string HtmlContent { get; set; }

    public string ContentPlainText { get; set; }

    public string Id { get; set; }

    public string Author { get; set; }

    public string Title { get; set; }

    public Uri Url { get; set; }

    public IList<string> Aliases { get; set; }

    public dynamic Data { get; set; }

    public DateTimeOffset? Date { get; set; }

    public DateTimeOffset? DateModified { get; set; }

    public DateTimeOffset? DatePublished { get; set; }

    public DateTimeOffset? DateExpired { get; set; }

    public DateTimeOffset? DateCreated { get; set; }

    public string Description { get; set; }

    public string Summary { get; set; }

    public bool Draft { get; set; }

    public FileModel File { get; set; }

    public IList<string> Keywords { get; set; }

    public PageKind Kind { get; set; }

    public string Lang { get; set; }

    public string? LinkTitle { get; set; }

    public string? Permalink { get; set; }

    public int? Weight { get; set; }

    public int WordCount { get; set; }

    public int WordCountFuzzy { get; set; }
    
    public IList<string> Categories { get; set; }

    public IList<string> Tags { get; set; }

    public int ReadingTime { get; set; }

    public MarkdownDocument? Document { get; internal set; }

    public IList<TocNode>? TableOfContents { get; internal set; }
}