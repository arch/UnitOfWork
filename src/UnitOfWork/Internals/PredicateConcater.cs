using System;
using System.Linq.Expressions;

namespace Arch.EntityFrameworkCore.UnitOfWork.Internals
{
    /// <summary>
    /// Class PredicateConcater.
    /// </summary>
    internal static class PredicateConcater
    {
        /// <summary>
        /// Ors the specified expr2.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">The expr1.</param>
        /// <param name="expr2">The expr2.</param>
        /// <returns>Expression&lt;Func&lt;T, System.Boolean&gt;&gt;.</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, secondBody), expr1.Parameters);
        }

        /// <summary>
        /// Ands the specified expr2.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">The expr1.</param>
        /// <param name="expr2">The expr2.</param>
        /// <returns>Expression&lt;Func&lt;T, System.Boolean&gt;&gt;.</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var secondBody = expr2.Body.Replace(expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, secondBody), expr1.Parameters);
        }

        /// <summary>
        /// Replaces the specified search ex.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="searchEx">The search ex.</param>
        /// <param name="replaceEx">The replace ex.</param>
        /// <returns>Expression.</returns>
        public static Expression Replace(this Expression expression, Expression searchEx, Expression replaceEx)
        {
            return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
        }

        /// <summary>
        /// Class ReplaceVisitor.
        /// Implements the <see cref="System.Linq.Expressions.ExpressionVisitor" />
        /// </summary>
        /// <seealso cref="System.Linq.Expressions.ExpressionVisitor" />
        private class ReplaceVisitor : ExpressionVisitor
        {
            /// <summary>
            /// From
            /// </summary>
            private readonly Expression _from, _to;

            /// <summary>
            /// Initializes a new instance of the <see cref="ReplaceVisitor"/> class.
            /// </summary>
            /// <param name="from">From.</param>
            /// <param name="to">To.</param>
            public ReplaceVisitor(Expression from, Expression to)
            {
                _from = from;
                _to = to;
            }

            /// <summary>
            /// Visits the specified node.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <returns>Expression.</returns>
            public override Expression Visit(Expression node)
            {
                return node == _from ? _to : base.Visit(node);
            }
        }
    }
}