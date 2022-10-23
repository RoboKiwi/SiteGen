using System.Xml;

namespace SiteGen.Core.Extensions;

public static class XmlWriterExtensions
{
    public static XmlWriter WriteElement(this XmlWriter writer, string name, object? value)
    {
        if (value == null) return writer;

        writer.WriteStartElement(name);
        writer.WriteString(value.ToString());
        writer.WriteEndElement();
        return writer;
    }
}
