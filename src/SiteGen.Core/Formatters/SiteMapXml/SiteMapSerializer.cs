using System;
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
            writer.WriteElement("lastmod", url.LastModified?.ToString("o"));
            writer.WriteElement("changefreq", url.ChangeFrequency?.ToString().ToLowerInvariant());
            writer.WriteElement("priority", url.Priority);

            writer.WriteEndElement();
        }

        writer.WriteEndElement();
        writer.WriteEndDocument();
    }

    internal static XmlReader CreateReader(string body)
    {
        // Trim leading whitespace
        body = body.TrimStart();

        // <!DOCTYPE declaration is case-sensitive.
        body = body.Replace("<!doctype", "<!DOCTYPE");

        var readerSettings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Ignore,
            CheckCharacters = false,
            IgnoreProcessingInstructions = true,
            NameTable = new NameTable()
        };

        // Configure to use the tolerant namespace manager, so if they have rogue namespaces,
        // we try to continue and parse the feed anyway.
        XmlNamespaceManager namespaceManager = new TolerantXmlNamespaceManager(readerSettings.NameTable);
        var context = new XmlParserContext(null, namespaceManager, "", XmlSpace.Preserve);
        var reader = XmlReader.Create(new StringReader(body), readerSettings, context);

        // This will move us to the root node.
        reader.MoveToContent();

        return reader;
    }

    class TolerantXmlNamespaceManager : XmlNamespaceManager
    {
        public TolerantXmlNamespaceManager(XmlNameTable nameTable) : base(nameTable) { }

        /// <summary>
        /// Gets the namespace URI for the specified prefix.
        /// </summary>
        /// <returns>
        /// Returns the namespace URI for <paramref name="prefix"/> or null if there is no mapped namespace. The returned string is atomized.For more information on atomized strings, see <see cref="T:System.Xml.XmlNameTable"/>.
        /// </returns>
        /// <param name="prefix">The prefix whose namespace URI you want to resolve. To match the default namespace, pass String.Empty. </param>
        public override string LookupNamespace(string prefix)
        {
            return base.LookupNamespace(prefix) ?? DefaultNamespace;
        }
    }

    public static IList<SiteMapXmlUrl> Deserialize(string xml)
    {
        using var reader = CreateReader(xml);
        return Deserialize(reader);
    }

    public static IList<SiteMapXmlUrl> Deserialize(XmlReader reader)
    {
        var results = new List<SiteMapXmlUrl>();

        reader.MoveToContent();

        reader.ReadStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

        while (reader.MoveToContent() != XmlNodeType.None)
        {
            if (reader.NodeType == XmlNodeType.EndElement)
            {
                //if (string.Equals("channel", reader.LocalName, StringComparison.OrdinalIgnoreCase))
                //{
                //    reader.ReadEndElement();
                //    continue;
                //}

                break;
            }

            switch (reader.LocalName.ToLowerInvariant())
            {
                case "url":
                    results.Add(ReadItem(reader));
                    continue;
            }

            reader.Skip();
        }

        return results;
    }

    internal static SiteMapXmlUrl ReadItem(XmlReader reader)
    {
        var url = new SiteMapXmlUrl();

        reader.MoveToContent();

        reader.ReadStartElement();

        for (reader.MoveToContent(); reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent())
        {
            if (reader.NodeType != XmlNodeType.Element) throw new XmlException("Only element node is expected under site map item.");

            switch (reader.LocalName.ToLowerInvariant())
            {
                case "loc":
                    var location = reader.ReadElementContentAsString();
                    url.Location = new Uri(location, UriKind.RelativeOrAbsolute);
                    continue;

                case "lastmod":
                    var lastmod = reader.ReadElementContentAsString();
                    url.LastModified = string.IsNullOrWhiteSpace(lastmod) ? null : DateTimeOffset.Parse(lastmod);
                    continue;

                case "changefreq":
                    var changeFreq = reader.ReadElementContentAsString();
                    url.ChangeFrequency = Enum.Parse<ChangeFrequency>(changeFreq);
                    continue;

                case "priority":
                    var priority = reader.ReadElementContentAsString();
                    url.Priority = double.Parse(priority);
                    continue;
            }

            //Trace.TraceInformation("Skipping unknown element: " + reader.LocalName);
            reader.Skip();
        }

        reader.ReadEndElement();

        return url;
    }
}