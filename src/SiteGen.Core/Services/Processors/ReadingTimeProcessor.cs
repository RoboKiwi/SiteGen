using SiteGen.Core.Models;

namespace SiteGen.Core.Services.Processors
{
    public class ReadingTimeProcessor : ISiteNodeProcessor
    {
        public Task ProcessAsync(SiteNode node, CancellationToken cancellationToken)
        {
            node.ReadingTime = (node.WordCount + 212) / 213;
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
