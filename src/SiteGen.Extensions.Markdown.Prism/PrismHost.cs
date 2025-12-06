using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Playwright;

namespace SiteGen.Extensions.Markdown.Prism;

public class PrismHost : IAsyncDisposable
{
    IPage page;
    readonly IServer server;
    readonly IServerAddressesFeature addresses;    
    static bool isInitialized;

    static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public PrismHost(IPage page, IServer server)
    {
        this.page = page;
        this.server = server;
        addresses = server.Features.Get<IServerAddressesFeature>()!;
    }

    public async ValueTask DisposeAsync()
    {
        await page.CloseAsync();
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
                    var address = addresses.Addresses.First();
                    var assembly = GetType().Assembly;
                    var name = assembly.GetName();
                    
                    var uri = new Uri(new Uri(address), $"/_content/{name.Name}/index.html");

                    await page.GotoAsync(uri.ToString());
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
