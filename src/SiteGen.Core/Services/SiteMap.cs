using SiteGen.Core.Models;
using SiteGen.Core.Services.Generators;
using System.Collections.ObjectModel;
using System.Net;

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
        var results = nodes.Where(x => x.Url == uri || WebUtility.UrlDecode(x.Url.ToString()) == WebUtility.UrlDecode(uri.ToString())).ToList();
        if (results.Count > 1)
        {
            throw new InvalidOperationException($"Multiple nodes found for URI '{uri}':{Environment.NewLine}{string.Join(Environment.NewLine + "\t", results.Select(x => x.Path))}");
        }
        return results.SingleOrDefault();
    }
}
