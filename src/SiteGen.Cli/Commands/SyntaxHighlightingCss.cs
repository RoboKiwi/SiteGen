using Microsoft.Extensions.Logging;
using SiteGen.Core;

namespace SiteGen.Cli.Commands;

class SyntaxHighlightingCss : ICommandlet
{
    private readonly IMonacoService monaco;
    private readonly ILogger<SyntaxHighlightingCss> logger;

    public SyntaxHighlightingCss(IMonacoService monaco, ILogger<SyntaxHighlightingCss> logger)
    {
        this.monaco = monaco;
        this.logger = logger;
    }

    public async Task ExecuteAsync()
    {
        logger.LogInformation("Generating syntax highlighting CSS...");
        var css = await monaco.GetCssAsync("vs-dark");
        Console.WriteLine(css);
    }
}