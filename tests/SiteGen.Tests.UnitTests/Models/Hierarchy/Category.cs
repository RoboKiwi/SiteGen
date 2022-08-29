using SiteGen.Core.Models;
using SiteGen.Core.Models.Hierarchy;

namespace SiteGen.Tests.UnitTests.Models.Hierarchy
{
    public class Category : ITreeEntity<Category>, IEntity
    {
        public Category(string name) : this()
        {
            Name = name;
        }

        public Category()
        {
            Id = Guid.NewGuid();
            Tree = new TreeInfo<Category>(this);
        }

        public TreeInfo<Category> Tree { get; set; }

        public string Name { get; set; }

        public Guid Id { get; set; }

        public override string ToString()
        {
            return string.Format("Category {0} [{1}]", Id, Name);
        }
    }
}