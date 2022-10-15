using Microsoft.Extensions.DependencyInjection;
using SiteGen.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SiteGen.Core.Services;

public class SiteMapService : ISiteMapService
{
    static readonly SemaphoreSlim locker = new SemaphoreSlim(1);

    private IList<SiteNode> nodes;

    private readonly SiteMapBuilder builder;

    public SiteMapService(SiteMapBuilder builder)
    {
        this.builder = builder;
    }

    public async Task<IList<SiteNode>> GetNodesAsync(string contentPath)
    {
        if (nodes == null)
        {
            try
            {
                await locker.WaitAsync();
                nodes ??= await builder.Build(contentPath);
            }
            finally
            {
                locker.Release();
            }
        }

        return nodes;
    }
}
