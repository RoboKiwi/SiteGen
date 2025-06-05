namespace SiteGen.Core.Services;

public interface ISiteMapBuilder
{
    Task<SiteMap> BuildAsync(CancellationToken cancellationToken);
}
