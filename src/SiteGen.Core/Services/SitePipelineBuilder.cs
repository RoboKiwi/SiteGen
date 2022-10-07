using SiteGen.Core.Models;

namespace SiteGen.Core.Services
{
    public class SitePipelineBuilder
    {
        readonly IList<ISiteNodeProcessor> processors = new List<ISiteNodeProcessor>();

        public SitePipelineBuilder(IServiceProvider services)
        {
            var processors = services.GetServices<ISiteNodeProcessor>()
                .ToList();

            foreach(var processor in processors)
            {
                this.processors.Add(processor);
            }
        }

        public async Task ProcessNode(SiteNode node)
        {
            foreach(var processor in processors)
            {
                await processor.ProcessAsync(node);
            }
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
