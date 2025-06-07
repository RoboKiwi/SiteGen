using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using SiteGen.Extensions.Markdown.Monaco;

namespace SiteGen.Tests.Extensions.Markdown;

[TestClass]
public class MonacoTests //: PageTest
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public async Task MonacoHighlighter()
    {
        using var playwright = await Playwright.CreateAsync();
        var options = new BrowserTypeLaunchOptions
        {
            Headless = true,
            Devtools = true,
            Channel = "msedge"
        };
        await using var browser = await playwright.Chromium.LaunchAsync(options);
        var contextOptions = new BrowserNewContextOptions { };
        var context = await browser.NewContextAsync(contextOptions);
        var page = await context.NewPageAsync();

        var directory = new DirectoryInfo(Path.Combine(TestContext.DeploymentDirectory, ".monaco"));
        await using var host = new MonacoHost(page, directory);
        var result = await host.Highlight("const test=\"value\";", "javascript");

        var expected = "<span><span class=\"mtk6\">const</span><span class=\"mtk1\"> test=</span><span class=\"mtk20\">\"value\"</span><span class=\"mtk1\">;</span></span><br/>";

        Assert.AreEqual(expected, result);
    }
}
