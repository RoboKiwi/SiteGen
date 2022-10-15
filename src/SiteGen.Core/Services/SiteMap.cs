using SiteGen.Core.Models;
using SiteGen.Core.Services.Generators;
using System.Collections.ObjectModel;

namespace SiteGen.Core.Services;

public class SiteMap : Collection<SiteNode>, ISiteMap
{
    protected override void InsertItem(int index, SiteNode item)
    {
        if (!Contains(item)) base.InsertItem(index, item);
    }

    public SiteNode Root
    {
        get
        {
            return this.FirstOrDefault() ?? throw new InvalidOperationException("No root node found");
        }
    }
}

public static class SiteMapExtensions
{
    public static SiteNode? FindByUri(this SiteMap nodes, Uri uri)
    {
        return nodes.SingleOrDefault(x => x.Url == uri);
    }
}
