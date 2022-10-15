namespace SiteGen.Core.Models.Hierarchy
{
    public static class TreeInfoExtensions
    {
        public static bool IsAncestorOf<T>(this TreeInfo<T> tree, T entity) where T : class, ITreeEntity<T>, IEntity
        {
            return entity.Tree.Ancestors.Contains(tree.Entity);
        }
    }
}