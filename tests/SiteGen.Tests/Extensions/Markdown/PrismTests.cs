using Microsoft.Playwright.MSTest;
using SiteGen.Extensions.Markdown.Prism;

namespace SiteGen.Tests.Extensions.Markdown
{
    [TestClass]
    public class PrismTests : PageTest
    {
        [TestMethod]
        public async Task Javascript()
        {
            var directory = new DirectoryInfo(TestContext.DeploymentDirectory);
            await using var host = new PrismHost(Page, directory);
            var result = await host.Highlight("const test=true;", "javascript");
            Assert.AreEqual(@"<span class=""token keyword"">const</span> test<span class=""token operator"">=</span><span class=""token boolean"">true</span><span class=""token punctuation"">;</span>", result);
        }
    }
}