using Microsoft.AspNetCore.Builder;

namespace SiteGen.Cli.Commands;

internal class ServeCommand : ICommandlet
{
    private readonly CliArgs settings;

    public ServeCommand(CliArgs settings)
    {
        this.settings = settings;
    }

    public async Task ExecuteAsync()
    {
        var directory = new DirectoryInfo(settings.ContentPath);

        var options = new WebApplicationOptions
        {
            WebRootPath = directory.FullName,
            ContentRootPath = directory.FullName,
        };
        var builder = WebApplication.CreateBuilder(options);
        var app = builder.Build();
        app.UseDefaultFiles().UseStaticFiles();
        await app.RunAsync();
    }
}
