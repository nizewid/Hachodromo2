using Hachodromo.Shared.DTOs;

namespace Hachodromo.API.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDto pagination)
        {
            if (pagination == null)
                throw new ArgumentNullException(nameof(pagination));
            return queryable
                .Skip((pagination.Page - 1) * pagination.RecordsNumber)
                .Take(pagination.RecordsNumber);
        }
    }
}
