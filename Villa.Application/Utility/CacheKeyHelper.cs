using System;
using System.Text.RegularExpressions;
using Villla.Domain.Common;

namespace Villla.Application.Utility
{
    public static class CacheKeyHelper
    {
        private static readonly Regex InvalidKeyCharacters = new("[^a-z0-9_]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string BuildPagedKey(string entityPrefix, PagedRequest request)
        {
            request.Normalize();

            var searchToken = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? "all"
                : NormalizeValue(request.SearchTerm);

            var sortColumn = string.IsNullOrWhiteSpace(request.SortColumn)
                ? "default"
                : NormalizeValue(request.SortColumn);

            var sortDirection = string.IsNullOrWhiteSpace(request.SortDirection)
                ? "asc"
                : NormalizeValue(request.SortDirection);

            return $"{entityPrefix}_page_{request.PageNumber}_size_{request.PageSize}_search_{searchToken}_sort_{sortColumn}_{sortDirection}";
        }

        public static string NormalizeValue(string value)
        {
            return InvalidKeyCharacters.Replace(value.Trim().ToLowerInvariant(), "_");
        }
    }
}
