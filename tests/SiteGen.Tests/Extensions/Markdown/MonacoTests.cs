using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using SiteGen.Extensions.Markdown.Monaco;

namespace SiteGen.Tests.Extensions.Markdown;

[TestClass, DoNotParallelize]
public class MonacoTests : PageTest
{
    public MonacoHost Host { get; private set; } = null!;


    [TestInitialize]
    public async Task InitializeAsync()
    {
        if(Host != null) return;
        var directory = new DirectoryInfo(Path.Combine(TestContext.DeploymentDirectory, ".monaco"));
        Host = new MonacoHost(Page, directory);
    }

    [TestCleanup]
    public async Task CleanupAsync()
    {
        //await Host.DisposeAsync();
    }

    [TestMethod]
    public async Task MonacoHighlighter()
    {        
        var result = await Host.Highlight("const test=\"value\";", "javascript");

        var expected = "<span><span class=\"mtk6\">const</span><span class=\"mtk1\"> test=</span><span class=\"mtk20\">\"value\"</span><span class=\"mtk1\">;</span></span><br/>";

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task MonacoGetCss()
    {        
        var result = await Host.GetCss("vs-dark");
        Assert.IsTrue(result.Contains("--vscode-editor-background: #1e1e1e;"));
    }
}
