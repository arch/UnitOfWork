using System;
using System.Linq;

namespace EfCore.UnitOfWork.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> PagedBy<T>(this IQueryable<T> queryable, PagingOptions options)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            if (options == null) throw new ArgumentNullException(nameof(options));

            return queryable.PagedBy(options.PageIndex, options.PageSize);
        }

        public static IQueryable<T> PagedBy<T>(this IQueryable<T> queryable, int pageIndex, int pageSize)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            return queryable.Skip(pageIndex * pageSize).Take(pageSize);
        }        
        
        public static IQueryable<TResult> ProjectTo<T, TResult>(this IQueryable<T> queryable, Func<T, TResult> projection)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            if (projection == null) throw new ArgumentNullException(nameof(projection));

            return queryable.Select(m => projection(m));
        }
    }
}