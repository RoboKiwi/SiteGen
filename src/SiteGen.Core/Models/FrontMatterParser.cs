using System.Text;

namespace SiteGen.Core.Models;

public class FrontMatterParser
{
    public static Tuple<FrontMatterFormat, string>? ReadBlock(string content)
    {
        using var reader = new StringReader(content);

        var line = reader.ReadLine()?.Trim();
            
        if (string.IsNullOrEmpty(line)) return null;

        var format = line switch
        {
            "---" => FrontMatterFormat.Yaml,
            "+++" => FrontMatterFormat.Toml,
            "{" => FrontMatterFormat.Json,
            _ => FrontMatterFormat.None
        };

        if(format == FrontMatterFormat.None) return null;

        // We're at the start of a front matter block

        // This will hold the front matter content to be parsed
        var sb = new StringBuilder();

        line = reader.ReadLine();
            
        while (line != null)
        {
            var closeDelimiter = line.Trim() switch
            {
                "---" => FrontMatterFormat.Yaml,
                "+++" => FrontMatterFormat.Toml,
                "}" => FrontMatterFormat.Json,
                _ => FrontMatterFormat.None
            };

            if (closeDelimiter == FrontMatterFormat.None)
            {
                // This line is part of the front matter contents
                sb.AppendLine(line);
            }

            line = reader.ReadLine();

            // End of the front matter?
            if (closeDelimiter != FrontMatterFormat.None)
            {
                // We can be forgiving about the closing delimiter, except
                // JSON needs a trailing empty line to truly mark the end of the object.
                if (closeDelimiter == FrontMatterFormat.Json && !string.IsNullOrEmpty(line))
                {
                    continue;
                }

                break;
            }
        }

        return new Tuple<FrontMatterFormat, string>(format, sb.ToString().Trim());
    }
}
