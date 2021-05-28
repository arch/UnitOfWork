//-----------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Arch team">
// Copyright (c) Arch team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Arch.EntityFrameworkCore.UnitOfWork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Query;
    using Arch.EntityFrameworkCore.UnitOfWork.Collections;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Defines the interfaces for generic repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Changes the table name. This require the tables in the same database.
        /// </summary>
        /// <param name="table"></param>
        /// <remarks>
        /// This only been used for supporting multiple tables in the same model. This require the tables in the same database.
        /// </remarks>
        void ChangeTable(string table);

        /// <summary>
        /// Gets the <see cref="IPagedList{TEntity}"/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="pageIndex">The index of page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        IPagedList<TEntity> GetPagedList(Expression<Func<TEntity, bool>> predicate = null,
                                         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                         int pageIndex = 0,
                                         int pageSize = 20,
                                         bool disableTracking = true,
                                         bool ignoreQueryFilters = false);

        /// <summary>
        /// Gets the <see cref="IPagedList{TEntity}"/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="pageIndex">The index of page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<IPagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> predicate = null,
                                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                    int pageIndex = 0,
                                                    int pageSize = 20,
                                                    bool disableTracking = true,
                                                    CancellationToken cancellationToken = default(CancellationToken),
                                                    bool ignoreQueryFilters = false);

        /// <summary>
        /// Gets the <see cref="IPagedList{TResult}"/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="pageIndex">The index of page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TResult}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        IPagedList<TResult> GetPagedList<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                  Expression<Func<TEntity, bool>> predicate = null,
                                                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                  int pageIndex = 0,
                                                  int pageSize = 20,
                                                  bool disableTracking = true,
                                                  bool ignoreQueryFilters = false) where TResult : class;

        /// <summary>
        /// Gets the <see cref="IPagedList{TEntity}"/> based on a predicate, orderby delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="pageIndex">The index of page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<IPagedList<TResult>> GetPagedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                                                             Expression<Func<TEntity, bool>> predicate = null,
                                                             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                             Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                             int pageIndex = 0,
                                                             int pageSize = 20,
                                                             bool disableTracking = true,
                                                             CancellationToken cancellationToken = default(CancellationToken),
                                                             bool ignoreQueryFilters = false) where TResult : class;

        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method defaults to a read-only, no-tracking query.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method defaults to a read-only, no-tracking query.</remarks>
        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
                                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                  bool disableTracking = true,
                                  bool ignoreQueryFilters = false);

        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method defaults to a read-only, no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method defaults to a read-only, no-tracking query.</remarks>
        TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                                           Expression<Func<TEntity, bool>> predicate = null,
                                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                           bool disableTracking = true,
                                           bool ignoreQueryFilters = false);

        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method defaults to a read-only, no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
        Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);

        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method defaults to a read-only, no-tracking query.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>Ex: This method defaults to a read-only, no-tracking query. </remarks>
        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);

        /// <summary>
        /// Uses raw SQL queries to fetch the specified <typeparamref name="TEntity" /> data.
        /// </summary>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An <see cref="IQueryable{TEntity}" /> that contains elements that satisfy the condition specified by raw SQL.</returns>
        IQueryable<TEntity> FromSql(string sql, params object[] parameters);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The found entity or null.</returns>
        TEntity Find(params object[] keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A <see cref="Task{TEntity}"/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
        ValueTask<TEntity> FindAsync(params object[] keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task{TEntity}"/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
        ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all entities. This method is not recommended
        /// </summary>
        /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Gets all entities. This method is not recommended
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null,
                                                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                  bool disableTracking = true,
                                                  bool ignoreQueryFilters = false);

        /// <summary>
        /// Gets all entities. This method is not recommended
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
        IQueryable<TResult> GetAll<TResult>(Expression<Func<TEntity,TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);

        /// <summary>
        /// Gets all entities. This method is not recommended
        /// </summary>
        /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
        Task<IList<TEntity>> GetAllAsync();

        /// <summary>
        /// Gets all entities. This method is not recommended
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
        Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,
                                                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                  bool disableTracking = true,
                                                  bool ignoreQueryFilters = false);

        /// <summary>
        /// Gets all entities. This method is not recommended
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>true</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
        Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);

        /// <summary>
        /// Gets the count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets async the count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets the long count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        long LongCount(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets async the long count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets the max based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        ///  /// <param name="selector"></param>
        /// <returns>decimal</returns>
        T Max<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        /// <summary>
        /// Gets the async max based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        ///  /// <param name="selector"></param>
        /// <returns>decimal</returns>
        Task<T> MaxAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        /// <summary>
        /// Gets the min based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <returns>decimal</returns>
        T Min<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        /// <summary>
        /// Gets the async min based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <returns>decimal</returns>
        Task<T> MinAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        /// <summary>
        /// Gets the average based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        ///  /// <param name="selector"></param>
        /// <returns>decimal</returns>
        decimal Average (Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        /// <summary>
            /// Gets the async average based on a predicate.
            /// </summary>
            /// <param name="predicate"></param>
            ///  /// <param name="selector"></param>
            /// <returns>decimal</returns>
        Task<decimal> AverageAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        /// <summary>
        /// Gets the sum based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        ///  /// <param name="selector"></param>
        /// <returns>decimal</returns>
        decimal Sum (Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        /// <summary>
        /// Gets the async sum based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        ///  /// <param name="selector"></param>
        /// <returns>decimal</returns>
        Task<decimal> SumAsync (Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        /// <summary>
        /// Gets the Exists record based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        bool Exists(Expression<Func<TEntity, bool>> selector = null);
        /// <summary>
        /// Gets the Async Exists record based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null);

        /// <summary>
        /// Inserts a new entity synchronously.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// Inserts a range of entities synchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        void Insert(params TEntity[] entities);

        /// <summary>
        /// Inserts a range of entities synchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Inserts a new entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
        ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Inserts a range of entities asynchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
        Task InsertAsync(params TEntity[] entities);

        /// <summary>
        /// Inserts a range of entities asynchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void Update(params TEntity[] entities);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes the entity by the specified primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        void Delete(object id);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void Delete(params TEntity[] entities);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Change entity state for patch method on web api.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// /// <param name="state">The entity state.</param>
        void ChangeEntityState(TEntity entity, EntityState state);
    }
}