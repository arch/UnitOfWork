// Copyright (c) Arch team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Arch.EntityFrameworkCore.UnitOfWork.Collections
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IPagedList{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the data to page</typeparam>
    public class PagedList<T> : IPagedList<T>
    {
        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        /// <value>The index of the page.</value>
        public int PageIndex { get; set; }
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; set; }
        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>The total count.</value>
        public int TotalCount { get; set; }
        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        /// <value>The total pages.</value>
        public int TotalPages { get; set; }
        /// <summary>
        /// Gets or sets the index from.
        /// </summary>
        /// <value>The index from.</value>
        public int IndexFrom { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<T> Items { get; set; }

        /// <summary>
        /// Gets the has previous page.
        /// </summary>
        /// <value>The has previous page.</value>
        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        /// <summary>
        /// Gets the has next page.
        /// </summary>
        /// <value>The has next page.</value>
        public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="indexFrom">The index from.</param>
        internal PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom)
        {
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
            }

            if (source is IQueryable<T> querable)
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                TotalCount = querable.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                Items = querable.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
            }
            else
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                TotalCount = source.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                Items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}" /> class.
        /// </summary>
        internal PagedList() => Items = new T[0];
    }


    /// <summary>
    /// Provides the implementation of the <see cref="IPagedList{T}"/> and converter.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    internal class PagedList<TSource, TResult> : IPagedList<TResult>
    {
        /// <summary>
        /// Gets the index of the page.
        /// </summary>
        /// <value>The index of the page.</value>
        public int PageIndex { get; }
        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; }
        /// <summary>
        /// Gets the total count.
        /// </summary>
        /// <value>The total count.</value>
        public int TotalCount { get; }
        /// <summary>
        /// Gets the total pages.
        /// </summary>
        /// <value>The total pages.</value>
        public int TotalPages { get; }
        /// <summary>
        /// Gets the index from.
        /// </summary>
        /// <value>The index from.</value>
        public int IndexFrom { get; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<TResult> Items { get; }

        /// <summary>
        /// Gets the has previous page.
        /// </summary>
        /// <value>The has previous page.</value>
        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        /// <summary>
        /// Gets the has next page.
        /// </summary>
        /// <value>The has next page.</value>
        public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{TSource, TResult}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="indexFrom">The index from.</param>
        public PagedList(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize, int indexFrom)
        {
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
            }

            if (source is IQueryable<TSource> querable)
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                TotalCount = querable.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                var items = querable.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

                Items = new List<TResult>(converter(items));
            }
            else
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                TotalCount = source.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                var items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

                Items = new List<TResult>(converter(items));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{TSource, TResult}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="converter">The converter.</param>
        public PagedList(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            PageIndex = source.PageIndex;
            PageSize = source.PageSize;
            IndexFrom = source.IndexFrom;
            TotalCount = source.TotalCount;
            TotalPages = source.TotalPages;

            Items = new List<TResult>(converter(source.Items));
        }
    }

    /// <summary>
    /// Provides some help methods for <see cref="IPagedList{T}"/> interface.
    /// </summary>
    public static class PagedList
    {
        /// <summary>
        /// Creates an empty of <see cref="IPagedList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type for paging </typeparam>
        /// <returns>An empty instance of <see cref="IPagedList{T}"/>.</returns>
        public static IPagedList<T> Empty<T>() => new PagedList<T>();
        /// <summary>
        /// Creates a new instance of <see cref="IPagedList{TResult}"/> from source of <see cref="IPagedList{TSource}"/> instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="converter">The converter.</param>
        /// <returns>An instance of <see cref="IPagedList{TResult}"/>.</returns>
        public static IPagedList<TResult> From<TResult, TSource>(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter) => new PagedList<TSource, TResult>(source, converter);
    }
}
