using SiteGen.Core.Models;

namespace SiteGen.Core.Services.Processors
{
    public class WordCountFuzzyProcessor : ISiteNodeProcessor
    {
        public Task ProcessAsync(SiteNode node, CancellationToken cancellationToken)
        {
            node.WordCountFuzzy = (node.WordCount + 100) / 100 * 100;
            return Task.CompletedTask;
        }
    }

    //public static class SiteNodeProcessor
    //{
    //    public static Task ProcessAsync()
    //    {
    //        ISiteNodeProcessor processor = null;

    //        var block = new ActionBlock<SiteNode>(processor.ProcessAsync);
    //    }
    //}
}
