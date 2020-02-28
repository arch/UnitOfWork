using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;

namespace Arch.EntityFrameworkCore.UnitOfWork
{
    /// <summary>
    /// Class PredicateBuilder. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class PredicateBuilder<T>
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static readonly PredicateBuilder<T> Instance = new PredicateBuilder<T>();

        /// <summary>
        /// Prevents a default instance of the <see cref="PredicateBuilder{T}"/> class from being created.
        /// </summary>
        private PredicateBuilder()
        {
        }

        /// <summary>
        /// Customs the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public PredicateWrap<T> Custom(Expression<Func<T, bool>> predicate) => predicate;

        /// <summary>
        /// Ins the specified selector.
        /// </summary>
        /// <typeparam name="TV">The type of the tv.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <param name="checkIn">The check in.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public PredicateWrap<T> In<TV>(Expression<Func<T, TV>> selector, IEnumerable<TV> checkIn)
        {
            var c = Expression.Constant(checkIn.WrapEnumerable());
            var p = selector.Parameters[0];
            var call = Expression.Call(typeof(Enumerable), "Contains", new[] { typeof(TV) }, c, selector.Body);
            var exp = Expression.Lambda<Func<T, bool>>(call, p);
            return exp;
        }

        /// <summary>
        /// Nots the in.
        /// </summary>
        /// <typeparam name="TV">The type of the tv.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <param name="checkIn">The check in.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public PredicateWrap<T> NotIn<TV>(Expression<Func<T, TV>> selector, IEnumerable<TV> checkIn)
        {
            var p = selector.Parameters[0];
            var values = Expression.Constant(checkIn.WrapEnumerable());
            var @in = Expression.Call(typeof(Enumerable), "Contains", new[] { typeof(TV) }, values, selector.Body);
            var not = Expression.Not(@in);
            var exp = Expression.Lambda<Func<T, bool>>(not, p);
            return exp;
        }

        /// <summary>
        /// Strings the contains.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="contains">The contains.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        /// <exception cref="SystemException">ILL SYSTEM LIB? string.Contains(string) NOT EXIST??</exception>
        public PredicateWrap<T> StringContains(Expression<Func<T, string>> selector, string contains)
        {
            if (string.IsNullOrWhiteSpace(contains)) return null;

            var stringContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })
                                       ?? throw new SystemException("ILL SYSTEM LIB? string.Contains(string) NOT EXIST??");

            var parameterExp = selector.Parameters[0];
            var valExp = Expression.Constant(contains, typeof(string));

            var callExp = Expression.Call(selector.Body, stringContainsMethod, valExp);
            var lambda = Expression.Lambda<Func<T, bool>>(callExp, parameterExp);

            return lambda;
        }

        /// <summary>
        /// Betweens the specified selector.
        /// </summary>
        /// <typeparam name="TV">The type of the tv.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="include">if set to <c>true</c> [include].</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public PredicateWrap<T> Between<TV>(Expression<Func<T, TV>> selector, TV min, TV max, bool include = true)
        {
            if (null == min && null == max) return null;

            PredicateWrap<T> predicateWrap = null;

            if (null != min)
            {
                predicateWrap = include
                    ? GreaterThanOrEqual(selector, min)
                    : GreaterThan(selector, min);
            }

            if (null != max)
            {
                predicateWrap &= include
                    ? LessThanOrEqual(selector, max)
                    : LessThan(selector, max);
            }

            return predicateWrap;
        }

        /// <summary>
        /// Equals the specified selector.(==)
        /// </summary>
        /// <typeparam name="TV">The type of the tv.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public PredicateWrap<T> Equal<TV>(Expression<Func<T, TV>> selector, TV valueToCompare) => BinOp(Expression.Equal, selector, valueToCompare);

        /// <summary>
        /// Nots the equal.(!=)
        /// </summary>
        /// <typeparam name="TV">The type of the tv.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public PredicateWrap<T> NotEqual<TV>(Expression<Func<T, TV>> selector, TV valueToCompare) => BinOp(Expression.NotEqual, selector, valueToCompare);

        /// <summary>
        /// Lesses the than.(<)
        /// </summary>
        /// <typeparam name="TV">The type of the tv.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public PredicateWrap<T> LessThan<TV>(Expression<Func<T, TV>> selector, TV valueToCompare) => BinOp(Expression.LessThan, selector, valueToCompare);

        /// <summary>
        /// Lesses the than or equal.(<=)
        /// </summary>
        /// <typeparam name="TV">The type of the tv.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public PredicateWrap<T> LessThanOrEqual<TV>(Expression<Func<T, TV>> selector, TV valueToCompare) => BinOp(Expression.LessThanOrEqual, selector, valueToCompare);

        /// <summary>
        /// Greaters the than.(>)
        /// </summary>
        /// <typeparam name="TV">The type of the tv.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public PredicateWrap<T> GreaterThan<TV>(Expression<Func<T, TV>> selector, TV valueToCompare) => BinOp(Expression.GreaterThan, selector, valueToCompare);

        /// <summary>
        /// Greaters the than or equal.(>=)
        /// </summary>
        /// <typeparam name="TV">The type of the tv.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        public PredicateWrap<T> GreaterThanOrEqual<TV>(Expression<Func<T, TV>> selector, TV valueToCompare) => BinOp(Expression.GreaterThanOrEqual, selector, valueToCompare);

        /// <summary>
        /// Bins the op.
        /// </summary>
        /// <typeparam name="TV">The type of the tv.</typeparam>
        /// <param name="op">The op.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="valueToCompare">The value to compare.</param>
        /// <returns>PredicateWrap&lt;T&gt;.</returns>
        private static PredicateWrap<T> BinOp<TV>(Func<Expression, Expression, Expression> op, Expression<Func<T, TV>> selector, TV valueToCompare)
        {
            var parameterExp = selector.Parameters[0];
            var valToCompareExp = Expression.Constant(valueToCompare, typeof(TV));

            var callExp = op(selector.Body, valToCompareExp);
            var lambda = Expression.Lambda<Func<T, bool>>(callExp, parameterExp);

            return lambda;
        }
    }
}
