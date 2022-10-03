using Microsoft.Extensions.Configuration;
using YamlDotNet.Core;

namespace SiteGen.Core.Configuration.Yaml;

/// <summary>
///     A YAML file based <see cref="FileConfigurationProvider"/>.
/// </summary>
public class YamlConfigurationProvider : FileConfigurationProvider
{
    /// <summary>
    ///     Initializes a new instance with the specified source.
    /// </summary>
    /// <param name="source">The source settings.</param>
    public YamlConfigurationProvider(YamlConfigurationSource source) : base(source) { }
        
    /// <summary>
    ///     Loads the YAML data from a stream.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    public override void Load(Stream stream)
    {
        try
        {
            Data = YamlConfigurationParser.Parse(stream);
        }
        catch (YamlException e)
        {
            throw new FormatException("Could not parse the YAML file.", e);
        }
    }

}