namespace SiteGen.Cli;

internal class CliArgs
{
    /// <summary>
    /// The path where static content is published and served from.
    /// </summary>
    public string ContentPath { get; set; }

    public Command Command { get; set; }

    public List<string> StaticPaths { get; set; }

    public string ServerUri { get; set; }
}
