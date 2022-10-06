namespace SiteGen.Core.Models;

/// <summary>
/// Defines an entity. An entity is simply a model object that
/// has a unique identifier of type <see cref="System.Guid"/>. If
/// the model object is stored in a database, <see cref="Guid"/>
/// would be the primary key.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    /// <value>The id.</value>
    Guid Guid { get; set; }
}