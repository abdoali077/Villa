using System;
using System.Linq.Expressions;

namespace Villla.Application.Utility
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceParameterVisitor(left.Parameters[0], parameter);
            var rightVisitor = new ReplaceParameterVisitor(right.Parameters[0], parameter);

            var leftBody = leftVisitor.Visit(left.Body);
            var rightBody = rightVisitor.Visit(right.Body);

            var body = Expression.OrElse(leftBody!, rightBody!);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceParameterVisitor(left.Parameters[0], parameter);
            var rightVisitor = new ReplaceParameterVisitor(right.Parameters[0], parameter);

            var leftBody = leftVisitor.Visit(left.Body);
            var rightBody = rightVisitor.Visit(right.Body);

            var body = Expression.AndAlso(leftBody!, rightBody!);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression? VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }
    }
}
