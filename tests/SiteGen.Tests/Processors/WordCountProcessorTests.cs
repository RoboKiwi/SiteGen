using SiteGen.Core.Models;
using SiteGen.Core.Services.Processors;

namespace SiteGen.Tests.Processors;

[TestClass]
public class WordCountProcessorTests
{
    [TestMethod]
    public async Task ProcessAsync()
    {
        var processor = new WordCountProcessor();

        var node = new SiteNode {
            ContentPlainText = " My content has five words. "
        };

        await processor.ProcessAsync(node, CancellationToken.None);

        Assert.AreEqual(5, node.WordCount);
    }
}