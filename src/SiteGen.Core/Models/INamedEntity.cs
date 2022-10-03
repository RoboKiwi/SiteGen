namespace SiteGen.Core.Models;

public interface INamedEntity : IEntity
{
    /// <summary>
    /// The name of the entity
    /// </summary>
    string Name { get; set; }
}
