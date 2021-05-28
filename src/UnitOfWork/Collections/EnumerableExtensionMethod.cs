using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arch.EntityFrameworkCore.UnitOfWork.Collections
{
    /// <summary>
    /// Class EnumerableExtensionMethod.
    /// </summary>
    public static class EnumerableExtensionMethod
    {
        /// <summary>
        /// 包裹泛型IEnumerable实例, 避免可能的数组实例造成EF动态过滤失败
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> WrapEnumerable<T>(this IEnumerable<T> source) => new EnumerableWrapper<T>(source);

        /// <summary>
        /// Class EnumerableWrapper.
        /// Implements the <see cref="System.Collections.Generic.IEnumerable{T}" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
        private class EnumerableWrapper<T> : IEnumerable<T>
        {
            /// <summary>
            /// The underlying instance
            /// </summary>
            private readonly IEnumerable<T> _underlyingInstance;

            /// <summary>
            /// Initializes a new instance of the <see cref="EnumerableWrapper{T}"/> class.
            /// </summary>
            /// <param name="underlyingInstance">The underlying instance.</param>
            public EnumerableWrapper(IEnumerable<T> underlyingInstance) => _underlyingInstance = underlyingInstance;

            /// <summary>
            /// 返回一个循环访问集合的枚举器。
            /// </summary>
            /// <returns>可用于循环访问集合的 <see cref="T:System.Collections.Generic.IEnumerator`1" />。</returns>
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => _underlyingInstance.GetEnumerator();

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>IEnumerator.</returns>
            IEnumerator IEnumerable.GetEnumerator() => _underlyingInstance.GetEnumerator();
        }
    }
}
