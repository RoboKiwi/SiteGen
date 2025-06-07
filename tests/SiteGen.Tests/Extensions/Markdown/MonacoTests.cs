using Microsoft.Playwright.MSTest;
using SiteGen.Extensions.Markdown.Monaco;

namespace SiteGen.Tests.Extensions.Markdown;

[TestClass]
public class MonacoTests : PageTest
{
    [TestMethod]
    public async Task MonacoHighlighter()
    {
        var directory = new DirectoryInfo(Path.Combine(TestContext.DeploymentDirectory, ".monaco"));
        await using var host = new MonacoHost(Page, directory);
        
        var result = await host.Highlight("const test=\"value\";", "javascript");

        var expected = "<span><span class=\"mtk6\">const</span><span class=\"mtk1\"> test=</span><span class=\"mtk20\">\"value\"</span><span class=\"mtk1\">;</span></span><br/>";

        Assert.AreEqual(expected, result);
    }
}
