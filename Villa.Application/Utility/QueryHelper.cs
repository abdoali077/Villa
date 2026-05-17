using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Villla.Domain.Common;

namespace Villla.Application.Utility
{
    public static class QueryHelper
    {
        public static Expression<Func<T, bool>>? BuildSearchPredicate<T>(string? searchTerm, params Expression<Func<T, string>>[] selectors)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || selectors == null || selectors.Length == 0)
                return null;

            searchTerm = searchTerm.Trim().ToLowerInvariant();
            Expression<Func<T, bool>>? predicate = null;

            foreach (var selector in selectors)
            {
                var member = selector.Body;
                var toLowerCall = Expression.Call(member, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);
                var searchExpression = Expression.Constant(searchTerm);
                var containsCall = Expression.Call(toLowerCall, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, searchExpression);
                var lambda = Expression.Lambda<Func<T, bool>>(containsCall, selector.Parameters);

                predicate = predicate == null ? lambda : predicate.Or(lambda);
            }

            return predicate;
        }

        public static Func<IQueryable<T>, IOrderedQueryable<T>>? BuildOrderBy<T>(PagedRequest request, IDictionary<string, (Func<IQueryable<T>, IOrderedQueryable<T>> Asc, Func<IQueryable<T>, IOrderedQueryable<T>> Desc)> sortMappings)
        {
            if (request == null || sortMappings == null || sortMappings.Count == 0)
                return null;

            request.Normalize();
            var sortKey = string.IsNullOrEmpty(request.SortColumn) ? "default" : request.SortColumn.Trim().ToLowerInvariant();

            if (!sortMappings.TryGetValue(sortKey, out var sortEntry))
                sortEntry = sortMappings.ContainsKey("default") && sortKey != "default"
                    ? sortMappings["default"]
                    : default;

            if (request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                return sortEntry.Desc;

            return sortEntry.Asc;
        }
    }
}
