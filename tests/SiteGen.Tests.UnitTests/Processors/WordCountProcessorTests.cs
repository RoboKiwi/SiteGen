using SiteGen.Core.Models;
using SiteGen.Core.Services.Processors;

namespace SiteGen.Tests.UnitTests.Processors
{
    public class WordCountProcessorTests
    {
        [Fact]
        public async Task ProcessAsync()
        {
            var processor = new WordCountProcessor();

            var node = new SiteNode {
                ContentPlainText = " My content has five words. "
            };

            await processor.ProcessAsync(node, CancellationToken.None);

            Assert.Equal(5, node.WordCount);
        }
    }
}
