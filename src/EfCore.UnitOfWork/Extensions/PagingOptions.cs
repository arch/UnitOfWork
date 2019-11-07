using System;

namespace UnitOfWork.Extensions
{
    public class PagingOptions
    {
        private const int DefaultPageSize = 20;

        public PagingOptions(int pageIndex, int pageSize = DefaultPageSize)
        {
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (pageIndex <= 0) throw new ArgumentOutOfRangeException(nameof(pageIndex));

            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public PagingOptions() : this(0, DefaultPageSize)
        {
        }

        public int PageIndex { get; }
        public int PageSize { get; }
    }
}