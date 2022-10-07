using SiteGen.Core.Models;

namespace SiteGen.Core.Services
{
    public interface ISiteMapService
    {
        Task<IList<SiteNode>> GetNodesAsync(string contentPath);
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
