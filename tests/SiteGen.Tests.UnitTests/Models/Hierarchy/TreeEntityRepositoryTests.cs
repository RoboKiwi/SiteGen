using SiteGen.Core.Models.Hierarchy;

namespace SiteGen.Tests.UnitTests.Models.Hierarchy
{
    public class TreeEntityRepositoryTests
    {
        IList<Category> flatList;
        IList<Category> repository;
        
        public TreeEntityRepositoryTests()
        {
            repository = CreateTree();
        }

        /// <summary>
        /// Creates a test tree hierarchy of the following levels:
        /// 
        /// Node1 (1,12)
        ///  Node1_1 (2,9)
        ///    Node1_1_1 (3,4)
        ///    Node1_1_2 (5,8)
        ///      Node1_1_2_1 (6,7)
        ///  Node1_2 (10,11)
        /// Node2 (13,14)
        /// Node3 (15,18)
        ///   Node3_1 (16,17)
        /// </summary>
        /// <returns></returns>
        IList<Category> CreateTree()
        {
            var list = new List<Category>(3);

            var node1 = new Category("Node1");
            var node2 = new Category("Node2");
            var node3 = new Category("Node3");

            list.Add(node1);
            list.Add(node2);
            list.Add(node3);

            var node1_1 = new Category("Node1_1");
            var node1_2 = new Category("Node1_2");

            node1.Tree.Children.Add(node1_1);
            node1.Tree.Children.Add(node1_2);

            var node1_1_1 = new Category("Node1_1_1");
            var node1_1_2 = new Category("Node1_1_2");

            node1_1.Tree.Children.Add(node1_1_1);
            node1_1.Tree.Children.Add(node1_1_2);

            var node1_1_2_1 = new Category("Node1_1_2_1");
            node1_1_2.Tree.Children.Add(node1_1_2_1);

            var node3_1 = new Category("Node3_1");
            node3.Tree.Children.Add(node3_1);

            flatList = new List<Category> { node1, node1_1, node1_1_1, node1_1_2, node1_1_2_1, node1_2, node2, node3, node3_1 };

            return flatList.RebuildTree().ToList();
        }

        [Fact]
        public void Siblings()
        {
            var results = repository.Siblings<Category>(flatList.Single(i => i.Name.Equals("Node1_1"))).ToList();
            Assert.Equal(1, results.Count);
            Assert.Equal("Node1_2", results[0].Name);

            results = repository.Siblings<Category>(flatList.Single(i => i.Name.Equals("Node1_1")), TreeListOptions.IncludeSelf).ToList();
            Assert.Equal(2, results.Count);
            Assert.Equal("Node1_1", results[0].Name);
            Assert.Equal("Node1_2", results[1].Name);

            results = repository.Siblings<Category>(flatList.Single(i => i.Name.Equals("Node1"))).ToList();
            Assert.Equal(2, results.Count);
            Assert.Equal("Node2", results[0].Name);
            Assert.Equal("Node3", results[1].Name);

            results = repository.Siblings<Category>(flatList.Single(i => i.Name.Equals("Node1")), TreeListOptions.IncludeSelf).ToList();
            Assert.Equal(3, results.Count);
            Assert.Equal("Node1", results[0].Name);
            Assert.Equal("Node2", results[1].Name);
            Assert.Equal("Node3", results[2].Name);
        }

        [Fact]
        public void Ancestors()
        {
            var ancestors = repository.Ancestors<Category>().ToList();
            Assert.Equal(9, ancestors.Count);
            Assert.Equal("Node1", ancestors[0].Name);
            Assert.Equal("Node1_1", ancestors[1].Name);
            Assert.Equal("Node1_1_1", ancestors[2].Name);
            Assert.Equal("Node1_1_2", ancestors[3].Name);
            Assert.Equal("Node1_1_2_1", ancestors[4].Name);
            Assert.Equal("Node1_2", ancestors[5].Name);
            Assert.Equal("Node2", ancestors[6].Name);
            Assert.Equal("Node3", ancestors[7].Name);
            Assert.Equal("Node3_1", ancestors[8].Name);
        }

        [Fact]
        public void Ancestors_for_specific_entity()
        {
            var ancestors = repository.Ancestors<Category>(flatList.Single(category => category.Name.Equals("Node1_1_2_1"))).ToList();
            Assert.Equal(3, ancestors.Count);
            Assert.Equal("Node1", ancestors[0].Name);
            Assert.Equal("Node1_1", ancestors[1].Name);
            Assert.Equal("Node1_1_2", ancestors[2].Name);
        }

        [Fact]
        public void Children()
        {
            Assert.Equal(9, repository.Count());
            var results = repository.Children<Category>(flatList.Single(i => i.Name.Equals("Node1")));
            Assert.Equal(2, results.Count());
            Assert.Equal("Node1_1", results.First().Name);
            Assert.Equal("Node1_2", results.Skip(1).Single().Name);
        }

        [Fact]
        public void Descendants()
        {
            Assert.Equal(9, repository.Count());
            var results = repository.Descendants<Category>(flatList.Single(i => i.Name.Equals("Node1"))).ToList();
            Assert.Equal(5, results.Count());
            Assert.Equal("Node1_1", results[0].Name);
            Assert.Equal("Node1_1_1", results[1].Name);
            Assert.Equal("Node1_1_2", results[2].Name);
            Assert.Equal("Node1_1_2_1", results[3].Name);
            Assert.Equal("Node1_2", results[4].Name);
        }

        [Fact]
        public void Parent()
        {
            Assert.Equal(9, repository.Count());
            var node = flatList.Single(i => i.Name.Equals("Node1_1_2_1"));
            Assert.Equal("Node1_1_2", node.Tree.Parent.Name);
            Assert.Equal("Node1_1", node.Tree.Parent.Tree.Parent.Name);
            Assert.Equal("Node1", node.Tree.Parent.Tree.Parent.Tree.Parent.Name);
            Assert.Null(node.Tree.Parent.Tree.Parent.Tree.Parent.Tree.Parent);
        }

        [Fact]
        public void RootNodes()
        {
            Assert.Equal(9, repository.Count());
            var results = repository.Children<Category>(null);
            Assert.Equal(3, results.Count());
        }
    }
}