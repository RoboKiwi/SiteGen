namespace SiteGen.Core.Models.Hierarchy
{
    /// <summary>
    /// Extension methods for a list of nodes that contains hierarchical entities of type <see cref="ITreeEntity{T}"/>.
    /// </summary>
    public static class TreeEntityRepositoryExtensions
    {
        /// <summary>
        /// Rebuilds the tree structure. This builds up the calculated <see cref="TreeInfo{T}.LeftValue"/> and
        /// <see cref="TreeInfo{T}.RightValue"/> values used to query the tree. All that's needed to reconstruct
        /// the tree is each entity's <see cref="TreeInfo{T}.Parent"/> value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity the repository handles.</typeparam>
        /// <param name="nodes">The list of nodes.</param>
        public static IEnumerable<TEntity> RebuildTree<TEntity>(this IEnumerable<TEntity> nodes) where TEntity : class, ITreeEntity<TEntity>, IEntity
        {
            // Get the root nodes
            var rootList = nodes.Where(x => x.Tree.Parent == null);

            Enumerable.Aggregate(rootList, 1, (current, item) => UpdateNode(item, current, nodes));

            return nodes;
        }

        static int UpdateNode<TEntity>(TEntity node, int leftValue, IEnumerable<TEntity> nodes) where TEntity : class, ITreeEntity<TEntity>, IEntity
        {
            node.Tree.LeftValue = leftValue;

            // A node with no children has a rightValue of leftValue + 1
            var rightValue = leftValue + 1;

            // If we have any children, process recursively, updating
            // the rightValue as we go.
            var children = nodes.Where(x => x.Tree.Parent != null && (x.Tree.Parent == node || (x.Tree.Parent.Guid != Guid.Empty && x.Tree.Parent.Guid.Equals(node.Guid))));

            rightValue = children.Aggregate(rightValue, (current, child) => UpdateNode(child, current, nodes));

            // Now we have our rightValue
            node.Tree.RightValue = rightValue;

            return rightValue + 1;
        }
    }
}