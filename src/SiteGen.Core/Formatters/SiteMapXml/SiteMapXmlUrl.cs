namespace SiteGen.Core.Formatters.SiteMapXml;

public class SiteMapXmlUrl
{
    public SiteMapXmlUrl()
    {

    }

    public SiteMapXmlUrl(string loc)
    {
        Location = new Uri(loc, UriKind.Relative);
    }

    public SiteMapXmlUrl(string loc, DateTimeOffset? lastModified) : this(loc)
    {
        LastModified = lastModified;
    }

    public Uri Location { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public ChangeFrequency? ChangeFrequency { get; set; }

    public double? Priority { get; set; }
}