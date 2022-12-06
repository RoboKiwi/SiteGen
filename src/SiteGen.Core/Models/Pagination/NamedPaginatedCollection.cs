namespace SiteGen.Core.Models.Pagination;

public class NamedPaginatedCollection<T> : PaginatedCollection<T>
{
    public NamedPaginatedCollection(ICollection<T> source, int page, int pageSize, Func<IEnumerable<T>, string> getPageNameFunction) : base(source, page, pageSize)
    {
        Paginate(source, getPageNameFunction);
    }

    void Paginate(ICollection<T> source, Func<IEnumerable<T>, string> function)
    {
        if (source.Count < RecordCount) throw new InvalidOperationException("You must load all results to do custom pagination names");

        pages.Clear();

        for (var i = 0; i < PageCount; i++)
        {
            pages.Add(function(source.Skip(i * PageSize).Take(PageSize)));
        }
    }
}