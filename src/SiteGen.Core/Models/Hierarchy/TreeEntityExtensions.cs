namespace SiteGen.Core.Models.Hierarchy
{
    public static class TreeEntityExtensions
    {
        public static IEnumerable<TEntity> FlattenTree<TEntity>(this TEntity entity) where TEntity : class, ITreeEntity<TEntity>, IEntity
        {
            if (entity == null) yield break;

            yield return entity;

            foreach(var child in entity.Tree.Children)
            {
                foreach(var node in child.FlattenTree())
                {
                    yield return node;
                }
            }
        }
    }
}