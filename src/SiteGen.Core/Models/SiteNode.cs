using SiteGen.Core.Models.Hierarchy;

namespace SiteGen.Core.Models;

public class SiteNode : IEntity, ITreeEntity<SiteNode>
{
    public Guid Id { get; set; }

    public NodeType Type { get; set; }

    public string Path { get; set; }

    public string FileName { get; set; }

    public string Ext { get; set; }

    public string Content { get; set; }

    public IDictionary<string, string> FrontMatter { get; set; }

    public ContentPageModel Metadata { get; set; }

    public TreeInfo<SiteNode> Tree { get; set; }

    public string HtmlContent { get; set; }
    public string ContentPlainText { get; set; }
}
