using System;

namespace Villla.Domain.Common
{
    public class PagedRequest
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;
        private const int MaxPageSize = 100;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : (value > MaxPageSize ? MaxPageSize : value);
        }

        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; }
        public string SortDirection { get; set; } = "asc";

        public void Normalize()
        {
            if (PageNumber < 1)
                PageNumber = 1;

            if (PageSize < 1)
                PageSize = 10;

            if (PageSize > MaxPageSize)
                PageSize = MaxPageSize;

            SortDirection = string.IsNullOrWhiteSpace(SortDirection)
                ? "asc"
                : SortDirection.Trim().ToLowerInvariant();

            SortColumn = string.IsNullOrWhiteSpace(SortColumn)
                ? null
                : SortColumn.Trim().ToLowerInvariant();

            SearchTerm = string.IsNullOrWhiteSpace(SearchTerm)
                ? null
                : SearchTerm.Trim();
        }
    }
}
