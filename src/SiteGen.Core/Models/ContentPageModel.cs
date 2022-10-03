namespace SiteGen.Core.Models;

public class ContentPageModel
{
    public string Id { get; set; }

    public string Guid { get; set; }

    public string Author { get; set; }

    public string Title { get; set; }

    public Uri Url { get; set; }

    public IList<string> Aliases { get; set; }

    public string Content { get; set; }

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

    public int Weight { get; set; }

    public int WordCount { get; set; }

    public int WordCountFuzzy { get; set; }


    public IList<string> Categories { get; set; }

    public IList<string> Tags { get; set; }
    public int ReadingTime { get; set; }
}
