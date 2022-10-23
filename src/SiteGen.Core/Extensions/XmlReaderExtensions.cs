using System.Xml;

namespace SiteGen.Core.Extensions;

static class XmlReaderExtensions
{
    public static bool IsTextNode(this XmlReader r)
    {
        switch (r.NodeType)
        {
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
            case XmlNodeType.Whitespace:
            case XmlNodeType.SignificantWhitespace:
                return true;
        }
        return false;
    }
}