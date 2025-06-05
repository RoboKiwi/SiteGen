using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Dynamic;
using YamlDotNet.Core.Tokens;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace SiteGen.Core.Configuration.Yaml;

public class YamlConfigurationParser
{
    YamlConfigurationParser() { }

    readonly Dictionary<string, string?> _data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
    readonly Stack<string> _paths = new Stack<string>();

    string[] NullValues { get; } = new[] { "~", "null", "" };

    public static IDictionary<string, string?> Parse(Stream input)
        => new YamlConfigurationParser().ParseStream(input);

    public IDictionary<string, string?> ParseStream(Stream stream)
    {
        using var reader = new StreamReader(stream);

        //var builder = new DeserializerBuilder().Build();

        //dynamic values = builder.Deserialize<ExpandoObject>(reader);

        //foreach(var entry in values)
        //{
        //    Traverse(entry);
        //}

        var yaml = new YamlStream();
        yaml.Load(reader);
        var rootNode = (YamlMappingNode)yaml.Documents[0].RootNode;
        Traverse(rootNode);

        return _data;
    }

    static string Clean(string str) => str?.Replace("_", string.Empty).Replace("-", string.Empty);

    void Traverse(KeyValuePair<object, object> entry, string path = null)
    {
        var fullPath = path == null ? entry.Key.ToString() : ConfigurationPath.Combine(path, entry.Key.ToString());
        if (entry.Value is Dictionary<object, object>)
        {
            var dictionary = (IDictionary<object,object>) entry.Value;
            foreach (var childEntry in dictionary)
            {
                Traverse(childEntry, fullPath);
            }
        }
        else if (entry.Value is IEnumerable<object>)
        {
            var enumerable = (IList<object>)entry.Value;

            for (int i = 0; i <  enumerable.Count; i++)
            {
                Traverse(enumerable[i].ToString(), ConfigurationPath.Combine(fullPath, i.ToString()));
            }
        }
        else
        {
            _data[Clean(fullPath)] = entry.Value.ToString();
        }
    }

    void Traverse(YamlNode root, string path = null)
    {
        static string Clean(string str) => str?.Replace("_", string.Empty).Replace("-", string.Empty);

        if (root is YamlScalarNode scalar)
        {
            if (_data.ContainsKey(Clean(path)))
            {
                throw new FormatException($"A duplicate key '{Clean(path)}' was found.");
            }

            var value = NullValues.Contains(scalar.Value.ToLower()) ? null : scalar.Value;

            if (value != null)
            {
                _data[Clean(path)] = NullValues.Contains(scalar.Value.ToLower()) ? null : scalar.Value;
            }
        }
        else if (root is YamlMappingNode map)
        {
            foreach (var node in map.Children)
            {
                var key = Clean(((YamlScalarNode)node.Key).Value);
                Traverse(node.Value, path == null ? key : ConfigurationPath.Combine(path, key));
            }
        }
        else if (root is YamlSequenceNode sequence)
        {
            for (int i = 0; i < sequence.Children.Count(); i++)
            {
                Traverse(sequence.Children[i], ConfigurationPath.Combine(path, i.ToString()));
            }
        }
    }

}