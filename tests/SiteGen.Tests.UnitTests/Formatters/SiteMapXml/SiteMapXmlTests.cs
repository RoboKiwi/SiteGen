using System.Text;
using System.Xml;
using SiteGen.Core.Formatters.SiteMapXml;

namespace SiteGen.Tests.UnitTests.Formatters.SiteMapXml;

public class SiteMapXmlTests
{
    [Fact]
    public void Serialize()
    {
        var sitemap = new List<SiteMapXmlUrl> { new("/", new DateTimeOffset(2022, 01, 02, 03, 04, 05, TimeSpan.FromHours(+12))) };

        var sb = new StringBuilder();
        var settings = new XmlWriterSettings
        {
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            OmitXmlDeclaration = false
        };

        using var writer = XmlWriter.Create(sb, settings);

        SiteMapSerializer.Serialize(writer, sitemap);

        writer.Flush();

        var xml = sb.ToString();

        Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">
<url>
<loc>/</loc>
<lastmod>2022-01-02T03:04:05.0000000+12:00</lastmod>
</url>
</urlset>".ReplaceLineEndings(""), xml);
    }
}