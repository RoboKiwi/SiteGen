using System;
using System.Buffers;
using System.IO;
using System.Reflection.Metadata;
using System.Xml.Linq;
using SiteGen.Core.Configuration;

namespace SiteGen.Core.Models;

/// <summary>
/// Defines an entity. An entity is simply a model object that
/// has a unique identifier of type <see cref="Guid"/>. If
/// the model object is stored in a database, <see cref="Id"/>
/// would be the primary key.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    /// <value>The id.</value>
    Guid Id { get; set; }
}
