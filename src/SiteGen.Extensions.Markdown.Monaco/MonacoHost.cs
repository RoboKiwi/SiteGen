using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Playwright;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using SiteGen.Core.Extensions;

namespace SiteGen.Extensions.Markdown.Monaco;

public class MonacoHost : IAsyncDisposable
{
    const string url = "http://127.0.0.1:0";
    readonly DirectoryInfo directory;
    IPage page;
    WebApplication app;
    static bool isInitialized;

    static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public MonacoHost(IPage page) : this(page, new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), ".monaco")))
    {
    }

    public MonacoHost(IPage page, DirectoryInfo directory)
    {
        this.directory = directory;
        this.page = page;

        var options = new WebApplicationOptions
        {
            WebRootPath = directory.FullName,
            ContentRootPath = directory.FullName,
            ApplicationName = "monaco"
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

    public async Task<string> GetCss(string theme)
    {
        await Initialize();
        return await page.EvaluateAsync<string>($@"(theme) => {{return getThemeCss(theme);}}", theme);
    }

    public async Task<string> Highlight(string source, string language)
    {
        await Initialize();

        // Set the source code
        return await page.EvaluateAsync<string>($@"(source) => {{
    return colorize(source, ""{language.ToLowerInvariant()}"", {{}});
}}", source);
    }

    private async Task Initialize()
    {
        if(!isInitialized)
        {
            await semaphore.WaitAsync();

            try
            {
                if(!isInitialized)
                {
                    // Purge the directory
                    directory.Delete(true);
                    directory.Create();

                    var type = GetType()!;
                    var resourceNameBase = type.Namespace! + ".dist";

                    // Write resources to the root directory
                    var assembly = Assembly.GetExecutingAssembly();
                    foreach(var name in assembly.GetManifestResourceNames())
                    {
                        var ext = Path.GetExtension(name);
                        var filename = Path.GetFileNameWithoutExtension(name.AsSpan().StripStart(resourceNameBase.AsSpan()).ToString()).TrimStart('.');

                        using var stream = assembly.GetManifestResourceStream(name);
                        using var destination = File.Open(Path.Combine(directory.FullName, $"{filename}{ext}"), FileMode.Create, FileAccess.Write);
                        await stream.CopyToAsync(destination);
                        await stream.FlushAsync();
                    }

                    await Task.Run(() => app.StartAsync());

                    // Get the address that was bound to (random port)
                    var server = app.Services.GetRequiredService<IServer>();
                    var addressFeature = server.Features.Get<IServerAddressesFeature>()!;
                    var address = addressFeature.Addresses.Single();

                    await page.GotoAsync(address);
                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForLoadStateAsync(LoadState.Load);

                    //await page.PauseAsync();

                    isInitialized = true;
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
