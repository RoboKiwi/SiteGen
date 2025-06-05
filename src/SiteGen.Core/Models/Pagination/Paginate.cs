namespace SiteGen.Core.Models.Pagination;

public static class PaginateExtensions
{
    public static PaginatedCollection<T> Paginate<T>(this IEnumerable<T> values)
    {
        return new PaginatedCollection<T>(values);
    }
}