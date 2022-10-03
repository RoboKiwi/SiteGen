using Microsoft.Extensions.Configuration;

namespace SiteGen.Core.Configuration.Yaml;

public class YamlStreamConfigurationSource : StreamConfigurationSource
{
    /// <summary>
    /// Builds the <see cref="YamlConfigurationProvider"/> for this source.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
    /// <returns>A <see cref="YamlConfigurationProvider"/></returns>
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new YamlStreamConfigurationProvider(this);
    }
}