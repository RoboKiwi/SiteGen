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

    IPage page;
    WebApplication app;
    static bool isInitialized;

    static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public PrismHost(IPage page)
    {
        this.page = page;

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
        await page.CloseAsync();
        await app.StopAsync();
        await app.DisposeAsync();
    }

    public async Task<string> Highlight(string source, string language)
    {
        if (!isInitialized)
        {
            await semaphore.WaitAsync();

            try
            {
                if (!isInitialized)
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

                    await Task.Run(() => app.StartAsync());
                                                            
                    // Get the address that was bound to (random port)
                    var server = app.Services.GetRequiredService<IServer>();
                    var addressFeature = server.Features.Get<IServerAddressesFeature>()!;
                    var address = addressFeature.Addresses.Single();

                    await page.GotoAsync(address);
                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                    isInitialized = true;
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        // Set the source code
        return await page.EvaluateAsync<string>($@"(source) => {{
    return Prism.highlight(source, Prism.languages.{language.ToLowerInvariant()}, ""{language.ToLowerInvariant()}"");
}}", source);
    }
}
