using Microsoft.Playwright;

// ReSharper disable once CheckNamespace
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
        // IPlaywright singleton factory
        services.AddSingleton(_ => Playwright.CreateAsync().GetAwaiter().GetResult());

        // IBrowser singleton for Chromium
        services.AddKeyedSingleton(nameof(BrowserType.Chromium), (sp, _) =>
        {
            var playwright = sp.GetRequiredService<IPlaywright>();
            return playwright.Chromium.LaunchAsync().GetAwaiter().GetResult();
        });

        // IPage transient factory for Chromium browser
        services.AddTransient(sp => {
            var browser = sp.GetRequiredKeyedService<IBrowser>(nameof(BrowserType.Chromium));
            return browser.NewPageAsync().GetAwaiter().GetResult();
        });

        return services;
    }
}
