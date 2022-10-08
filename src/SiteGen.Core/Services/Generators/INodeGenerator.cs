namespace SiteGen.Core.Services.Generators;

public interface INodeGenerator
{
    Task GenerateAsync(SiteMap nodes);
}
