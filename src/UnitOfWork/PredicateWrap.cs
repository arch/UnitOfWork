using System;
using System.Linq.Expressions;
using Arch.EntityFrameworkCore.UnitOfWork.Internals;

namespace Arch.EntityFrameworkCore.UnitOfWork
{
    /// <summary>
    /// Class PredicateWrap.
    /// </summary>
    public static class PredicateWrap
    {
        /// <summary>
        /// Ops the specified exp.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp">The exp.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public static PredicateWrap<T> Op<T>(this Expression<Func<T, bool>> exp) => new PredicateWrap<T>(exp);
    }

    /// <summary>
    /// Class PredicateWrap.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PredicateWrap<T>
    {
        /// <summary>
        /// The expression
        /// </summary>
        private readonly Expression<Func<T, bool>> _expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateWrap{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        internal PredicateWrap(Expression<Func<T, bool>> expression) => _expression = expression;

        /// <summary>
        /// Whens the specified condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public PredicateWrap<T> When(bool condition) => condition ? this : null;

        /// <summary>
        /// Performs an implicit conversion from <see cref="PredicateWrap{T}"/> to Expression.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Expression<Func<T, bool>>(PredicateWrap<T> me)
        {
            var expression = me?._expression;
            if (true == expression?.CanReduce) return (Expression<Func<T, bool>>)expression.Reduce();
            return expression;
        }

        /// <summary>
        /// Performs an implicit conversion from Expression to <see cref="PredicateWrap{T}"/>.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator PredicateWrap<T>(Expression<Func<T, bool>> me) => me == null ? null : new PredicateWrap<T>(me);

        /// <summary>
        /// Implements the &amp; operator.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static PredicateWrap<T> operator &(PredicateWrap<T> a, PredicateWrap<T> b)
        {
            var aIsNull = null == a?._expression;
            var bIsNull = null == b?._expression;

            if (aIsNull && bIsNull) return null;

            if (aIsNull) return b;
            if (bIsNull) return a;

            return new PredicateWrap<T>(a._expression.And(b._expression));
        }

        /// <summary>
        /// Implements the &amp; operator.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static PredicateWrap<T> operator &(PredicateWrap<T> a, Expression<Func<T, bool>> b)
        {
            var aIsNull = null == a?._expression;
            var bIsNull = null == b;

            if (aIsNull && bIsNull) return null;

            if (aIsNull) return b;
            if (bIsNull) return a;

            return new PredicateWrap<T>(a._expression.And(b));
        }

        /// <summary>
        /// Implements the &amp; operator.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static PredicateWrap<T> operator &(Expression<Func<T, bool>> a, PredicateWrap<T> b)
        {
            var aIsNull = null == a;
            var bIsNull = null == b?._expression;

            if (aIsNull && bIsNull) return null;

            if (aIsNull) return b;
            if (bIsNull) return a;

            return new PredicateWrap<T>(a.And(b._expression));
        }

        /// <summary>
        /// Implements the | operator.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static PredicateWrap<T> operator |(PredicateWrap<T> a, PredicateWrap<T> b)
        {
            var aIsNull = null == a?._expression;
            var bIsNull = null == b?._expression;

            if (aIsNull && bIsNull) return null;

            if (aIsNull) return b;
            if (bIsNull) return a;

            return new PredicateWrap<T>(a._expression.Or(b._expression));
        }

        /// <summary>
        /// Implements the | operator.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static PredicateWrap<T> operator |(PredicateWrap<T> a, Expression<Func<T, bool>> b)
        {
            var aIsNull = null == a?._expression;
            var bIsNull = null == b;

            if (aIsNull && bIsNull) return null;

            if (aIsNull) return b;
            if (bIsNull) return a;

            return new PredicateWrap<T>(a._expression.Or(b));
        }

        /// <summary>
        /// Implements the | operator.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static PredicateWrap<T> operator |(Expression<Func<T, bool>> a, PredicateWrap<T> b)
        {
            var aIsNull = null == a;
            var bIsNull = null == b?._expression;

            if (aIsNull && bIsNull) return null;

            if (aIsNull) return b;
            if (bIsNull) return a;

            return new PredicateWrap<T>(a.Or(b._expression));
        }
    }
}
