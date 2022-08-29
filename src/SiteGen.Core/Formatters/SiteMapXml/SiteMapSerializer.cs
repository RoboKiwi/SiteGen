using System.Xml;
using SiteGen.Core.Extensions;

namespace SiteGen.Core.Formatters.SiteMapXml;

public class SiteMapSerializer
{
    public static void Serialize(XmlWriter writer, IEnumerable<SiteMapXmlUrl> siteMap)
    {
        writer.WriteStartDocument(true);
        writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            
        foreach (var url in siteMap)
        {
            writer.WriteStartElement("url");

            writer.WriteElement("loc", url.Location);
            writer.WriteElement("lastmod", url.LastModified.ToString("o"));
            writer.WriteElement("changefreq", url.ChangeFrequency?.ToString().ToLowerInvariant());
            writer.WriteElement("priority", url.Priority);

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();
    }
}