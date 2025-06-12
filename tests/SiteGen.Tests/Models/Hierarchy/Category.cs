using SiteGen.Core.Models;
using SiteGen.Core.Models.Hierarchy;

namespace SiteGen.Tests.Models.Hierarchy;

public class Category : ITreeEntity<Category>, IEntity
{
    public Category(string name) : this()
    {
        Name = name;
    }

    public Category()
    {
        Guid = Guid.NewGuid();
        Tree = new TreeInfo<Category>(this);
    }

    public TreeInfo<Category> Tree { get; set; }

    public string? Name { get; set; }

    public Guid Guid { get; set; }

    public override string ToString()
    {
        return string.Format("Category {0} [{1}]", Guid, Name);
    }
}