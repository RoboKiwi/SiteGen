namespace SiteGen.Core.Models;

[Flags]
public enum FrontMatterFormat
{
    None = 0,
    Yaml = 1,
    Json = 2,
    Toml = 4
}
