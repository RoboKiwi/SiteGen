using SiteGen.Core.Models;

namespace SiteGen.Core.Services;

/// <summary>
/// Contract for a service that can consume a <see cref="SiteNode"/>
/// for processing.
/// </summary>
public interface ISiteNodeProcessor
{
    Task ProcessAsync(SiteNode node, CancellationToken cancellationToken);
}