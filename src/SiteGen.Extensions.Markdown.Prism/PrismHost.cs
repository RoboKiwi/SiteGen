using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Playwright;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace SiteGen.Extensions.Markdown.Prism;

public class PrismHost : IAsyncDisposable
{
    const string url = "http://127.0.0.1:0";
    readonly DirectoryInfo directory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), ".prism"));

    IBrowser browser;
    IPage page;
    WebApplication app;

    static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public PrismHost([FromKeyedServices(nameof(BrowserType.Chromium))]IBrowser browser)
    {
        this.browser = browser;

        var options = new WebApplicationOptions
        {
            WebRootPath = directory.FullName,
            ContentRootPath = directory.FullName,
            ApplicationName = "prism"
        };

        if (!directory.Exists) directory.Create();

        var builder = WebApplication.CreateSlimBuilder(options);

        builder.WebHost.UseUrls(url);

        app = builder.Build();

        app
            .UseDefaultFiles()
            .UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true });
    }

    public async ValueTask DisposeAsync()
    {
        await browser.DisposeAsync();
        await app.DisposeAsync();
    }

    public async Task<string> Highlight(string source, string language)
    {
        if (page == null)
        {
            await semaphore.WaitAsync();

            try
            {
                if (page == null)
                {
                    // Write resources to the root directory
                    var assembly = Assembly.GetExecutingAssembly();
                    foreach (var name in assembly.GetManifestResourceNames())
                    {
                        var ext = Path.GetExtension(name);
                        var filename = Path.GetExtension(Path.GetFileNameWithoutExtension(name)).TrimStart('.');

                        using var stream = assembly.GetManifestResourceStream(name);
                        using var destination = File.Open(Path.Combine(directory.FullName, $"{filename}{ext}"), FileMode.Create, FileAccess.Write);
                        stream.CopyTo(destination);
                        stream.Flush();
                    }

                    Task.Run(() => app.RunAsync());

                    page = await browser.NewPageAsync();

                    var server = app.Services.GetRequiredService<IServer>();
                    var addressFeature = server.Features.Get<IServerAddressesFeature>()!;

                    await page.GotoAsync(addressFeature.Addresses.Single());
                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        // Load the source
        var expression = $"Prism.highlight(`{source}`, Prism.languages.{language}, \"{language}\");";
        var colorized = await page.EvaluateAsync<string>(expression);

        return colorized;
    }
}
