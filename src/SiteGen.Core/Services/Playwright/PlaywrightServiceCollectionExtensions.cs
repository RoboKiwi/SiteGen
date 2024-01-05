using Microsoft.Playwright;

namespace SiteGen.Core;

public static class PlaywrightServiceCollectionExtensions
{
    /// <summary>
    /// Injects singletons for <see cref="IPlaywright"/> and <see cref="IBrowser"/>, using
    /// <see cref="BrowserType.Chromium"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigurePlaywright(this IServiceCollection services)
    {
        services.AddSingleton((sp) =>
        {
            return Playwright.CreateAsync().GetAwaiter().GetResult();
        });

        services.AddKeyedSingleton(nameof(BrowserType.Chromium), (sp, key) =>
        {
            var playwright = sp.GetRequiredService<IPlaywright>();
            return playwright.Chromium.LaunchAsync().GetAwaiter().GetResult();
        });

        services.AddTransient((sp) => {
            var browser = sp.GetRequiredKeyedService<IBrowser>(nameof(BrowserType.Chromium));
            return browser.NewPageAsync().GetAwaiter().GetResult();
        });

        return services;
    }
}
