using System;
using System.Collections.Generic;
using System.Text;

namespace MyAOS.Domain.Dto
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }

        // Computed property to help front-end pagination
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
