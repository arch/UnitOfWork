using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore
{
    public static class IQueryablePageListExtensions
    {
        /// <summary>
        /// Converts the specified source to <see cref="IPagedList{T}"/> by the specified <paramref name="pageIndex"/> and <paramref name="pageSize"/>.
        /// </summary>
        /// <typeparam name="T">The type of the source.</typeparam>
        /// <param name="source">The source to paging.</param>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="indexFrom">The start index value.</param>
        /// <returns>An instance of the inherited from <see cref="IPagedList{T}"/> interface.</returns>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, int indexFrom = 0)
        {

            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - indexFrom) * pageSize).Take(pageSize).ToListAsync();

            var pagedList = new PagedList<T>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                IndexFrom = indexFrom,
                TotalCount = count,
                Items = items
            };

            return pagedList;
        }

        /// <summary>
        /// Converts the specified source to <see cref="IPagedList{T}"/> by the specified <paramref name="converter"/>, <paramref name="pageIndex"/> and <paramref name="pageSize"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="source">The source to convert.</param>
        /// <param name="converter">The converter to change the <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="indexFrom">The start index value.</param>
        /// <returns>An instance of the inherited from <see cref="IPagedList{T}"/> interface.</returns>
        public static async Task<IPagedList<TResult>> ToPagedListAsync<TSource, TResult>(this IQueryable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize, int indexFrom = 0)
        {
            var sorcePage = await ToPagedListAsync(source: source, pageIndex: pageIndex, pageSize: pageSize, indexFrom: indexFrom);
            return PagedList.From<TResult, TSource>(sorcePage, converter);

        }
    }
}
