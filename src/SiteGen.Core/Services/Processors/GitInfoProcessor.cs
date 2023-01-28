using SiteGen.Core.Models;
using System.Diagnostics;

namespace SiteGen.Core.Services.Processors;

/// <summary>
/// Updates the node front matter and metadata with
/// information from its last Git commit.
/// </summary>
public class GitInfoProcessor : ISiteNodeProcessor
{
    public async Task ProcessAsync(SiteNode node)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"log -1 --pretty=format:\"%aI '%an' '%ae'\" \"{node.Path}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = Process.Start(startInfo)!;
        process.WaitForExit(1000);

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        var chunks = output.Trim().Split(' ');

        if(!string.IsNullOrWhiteSpace(error) || chunks.Length == 0 || string.IsNullOrWhiteSpace(chunks[0]))
        {
            return;
        }

        var authorDate = DateTimeOffset.Parse(chunks[0]);

        node.Date ??= authorDate;
        node.DateModified = authorDate;
    }
}