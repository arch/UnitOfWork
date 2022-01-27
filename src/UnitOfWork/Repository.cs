// Copyright (c) Arch team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Arch.EntityFrameworkCore.UnitOfWork
{
    /// <summary>
    /// Represents a default generic repository implements the <see cref="IRepository{TEntity}"/> interface.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext DbContext;
        protected readonly DbSet<TEntity> DbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public Repository(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            DbSet = DbContext.Set<TEntity>();
        }

        /// <summary>
        /// Changes the table name. This require the tables in the same database.
        /// </summary>
        /// <param name="table"></param>
        /// <remarks>
        /// This only been used for supporting multiple tables in the same model. This require the tables in the same database.
        /// </remarks>
        public virtual void ChangeTable(string table)
        {
            if (DbContext.Model.FindEntityType(typeof(TEntity)) is IConventionEntityType relational)
            {
                relational.SetTableName(table);
            }
        }

        /// <summary>
        /// Gets all entities. This method is not recommended
        /// </summary>
        /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
        public IQueryable<TEntity> GetAll() => DbSet;

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
        public IQueryable<TEntity> GetAll(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }

            return query;
        }

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
        public IQueryable<TResult> GetAll<TResult>(
            Expression<Func<TEntity,TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector);
            }

            return query.Select(selector);
        }

        /// <summary>
        /// Gets the <see cref="IPagedList{TEntity}"/> based on a predicate, orderBy delegate and page information. This method default no-tracking query.
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
        public virtual IPagedList<TEntity> GetPagedList(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).ToPagedList(pageIndex, pageSize);
            }

            return query.ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets the <see cref="IPagedList{TEntity}"/> based on a predicate, orderBy delegate and page information. This method default no-tracking query.
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
        public virtual Task<IPagedList<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true,
            CancellationToken cancellationToken = default,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }

            return query.ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
        }

        /// <summary>
        /// Gets the <see cref="IPagedList{TResult}"/> based on a predicate, orderBy delegate and page information. This method default no-tracking query.
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
        public virtual IPagedList<TResult> GetPagedList<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true,
            bool ignoreQueryFilters = false)
            where TResult : class
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).ToPagedList(pageIndex, pageSize);
            }

            return query.Select(selector).ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets the <see cref="IPagedList{TEntity}"/> based on a predicate, orderBy delegate and page information. This method default no-tracking query.
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
        public virtual Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true,
            CancellationToken cancellationToken = default,
            bool ignoreQueryFilters = false)
            where TResult : class
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
            }

            return query.Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
        }

        /// <summary>
        /// Gets the first or default entity based on a predicate, orderBy delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public virtual TEntity GetFirstOrDefault(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).FirstOrDefault();
            }

            return query.FirstOrDefault();
        }


        /// <inheritdoc />
        public virtual async Task<TEntity> GetFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).FirstOrDefaultAsync();
            }

            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the first or default entity based on a predicate, orderBy delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public virtual TResult GetFirstOrDefault<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).FirstOrDefault();
            }

            return query.Select(selector).FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual async Task<TResult> GetFirstOrDefaultAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).Select(selector).FirstOrDefaultAsync();
            }

            return await query.Select(selector).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Uses raw SQL queries to fetch the specified <typeparamref name="TEntity" /> data.
        /// </summary>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An <see cref="IQueryable{TEntity}" /> that contains elements that satisfy the condition specified by raw SQL.</returns>
        public virtual IQueryable<TEntity> FromSql(string sql, params object[] parameters) 
            => DbSet.FromSqlRaw(sql, parameters);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The found entity or null.</returns>
        public virtual TEntity Find(params object[] keyValues) 
            => DbSet.Find(keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A <see cref="Task{TEntity}" /> that represents the asynchronous insert operation.</returns>
        public virtual ValueTask<TEntity> FindAsync(params object[] keyValues) 
            => DbSet.FindAsync(keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task{TEntity}"/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
        public virtual ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken) => DbSet.FindAsync(keyValues, cancellationToken);

        /// <summary>
        /// Gets the count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? DbSet.Count() : DbSet.Count(predicate);

        /// <summary>
        /// Gets async the count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? await DbSet.CountAsync() : await DbSet.CountAsync(predicate);

        /// <summary>
        /// Gets the long count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? DbSet.LongCount() : DbSet.LongCount(predicate);

        /// <summary>
        /// Gets async the long count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? await DbSet.LongCountAsync() : await DbSet.LongCountAsync(predicate);

        /// <summary>
        /// Gets the max based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// ///
        /// <returns>decimal</returns>
        public virtual T Max<T>(Expression<Func<TEntity, T>> selector, Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? DbSet.Max(selector) : DbSet.Where(predicate).Max(selector);

        /// <summary>
        /// Gets the async max based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// ///
        /// <returns>decimal</returns>
        public virtual async Task<T> MaxAsync<T>(Expression<Func<TEntity, T>> selector,
            Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? await DbSet.MaxAsync(selector) : await DbSet.Where(predicate).MaxAsync(selector);

        /// <summary>
        /// Gets the min based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// ///
        /// <returns>decimal</returns>
        public virtual T Min<T>(Expression<Func<TEntity, T>> selector, Expression<Func<TEntity, bool>> predicate = null)
            => predicate == null ? DbSet.Min(selector) : DbSet.Where(predicate).Min(selector);

        /// <summary>
        /// Gets the async min based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// ///
        /// <returns>decimal</returns>
        public virtual async Task<T> MinAsync<T>(Expression<Func<TEntity, T>> selector,
            Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? await DbSet.MinAsync(selector) : await DbSet.Where(predicate).MinAsync(selector);

        /// <summary>
        /// Gets the average based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// ///
        /// <returns>decimal</returns>
        public virtual decimal Average(Expression<Func<TEntity, decimal>> selector,
            Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? DbSet.Average(selector) : DbSet.Where(predicate).Average(selector);

        /// <summary>
        /// Gets the async average based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// ///
        /// <returns>decimal</returns>
        public virtual async Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector,
            Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? await DbSet.AverageAsync(selector) : await DbSet.Where(predicate).AverageAsync(selector);

        /// <summary>
        /// Gets the sum based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// ///
        /// <returns>decimal</returns>
        public virtual decimal Sum(Expression<Func<TEntity, decimal>> selector,
            Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? DbSet.Sum(selector) : DbSet.Where(predicate).Sum(selector);

        /// <summary>
        /// Gets the async sum based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// ///
        /// <returns>decimal</returns>
        public virtual async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector,
            Expression<Func<TEntity, bool>> predicate = null) 
            => predicate == null ? await DbSet.SumAsync(selector) : await DbSet.Where(predicate).SumAsync(selector);

        /// <summary>
        /// Gets the exists based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public bool Exists(Expression<Func<TEntity, bool>> selector = null) 
            => selector == null ? DbSet.Any() : DbSet.Any(selector);

        /// <summary>
        /// Gets the async exists based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null) 
            => selector == null ? await DbSet.AnyAsync() : await DbSet.AnyAsync(selector);

        /// <summary>
        /// Inserts a new entity synchronously.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        public virtual TEntity Insert(TEntity entity) 
            => DbSet.Add(entity).Entity;

        /// <summary>
        /// Inserts a range of entities synchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        public virtual void Insert(params TEntity[] entities) 
            => DbSet.AddRange(entities);

        /// <summary>
        /// Inserts a range of entities synchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        public virtual void Insert(IEnumerable<TEntity> entities) 
            => DbSet.AddRange(entities);

        /// <summary>
        /// Inserts a new entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
        public virtual ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
            => DbSet.AddAsync(entity, cancellationToken);

        // Shadow properties?
        //var property = _dbContext.Entry(entity).Property("Created");
        //if (property != null) {
        //property.CurrentValue = DateTime.Now;
        //}
        /// <summary>
        /// Inserts a range of entities asynchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <returns>A <see cref="Task" /> that represents the asynchronous insert operation.</returns>
        public virtual Task InsertAsync(params TEntity[] entities) 
            => DbSet.AddRangeAsync(entities);

        /// <summary>
        /// Inserts a range of entities asynchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
        public virtual Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) 
            => DbSet.AddRangeAsync(entities, cancellationToken);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Update(TEntity entity) 
            => DbSet.Update(entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void UpdateAsync(TEntity entity) 
            => DbSet.Update(entity);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public virtual void Update(params TEntity[] entities) 
            => DbSet.UpdateRange(entities);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public virtual void Update(IEnumerable<TEntity> entities)
            => DbSet.UpdateRange(entities);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual void Delete(TEntity entity) 
            => DbSet.Remove(entity);

        /// <summary>
        /// Deletes the entity by the specified primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        public virtual void Delete(object id)
        {
            // using a stub entity to mark for deletion
            var typeInfo = typeof(TEntity).GetTypeInfo();
            var key = DbContext.Model.FindEntityType(typeInfo).FindPrimaryKey().Properties.FirstOrDefault();
            var property = typeInfo.GetProperty(key?.Name);
            if (property != null)
            {
                var entity = Activator.CreateInstance<TEntity>();
                property.SetValue(entity, id);
                DbContext.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                var entity = DbSet.Find(id);
                if (entity != null)
                {
                    Delete(entity);
                }
            }
        }

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public virtual void Delete(params TEntity[] entities) 
            => DbSet.RemoveRange(entities);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public virtual void Delete(IEnumerable<TEntity> entities) 
            => DbSet.RemoveRange(entities);

        /// <summary>
        /// Gets all entities. This method is not recommended
        /// </summary>
        /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
        public async Task<IList<TEntity>> GetAllAsync() 
            => await DbSet.ToListAsync();

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
        public async Task<IList<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> predicate = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, 
            bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }
		
		private bool ExistsUpdateTimestamp(TEntity entity, out TEntity entityForUpdate)
        {
            IEntityType entityType = DbContext.Model.FindEntityType(typeof(TEntity).ToString());
            IKey key = entityType.FindPrimaryKey();
            object[] objArr = key.Properties.Select(q => entity.GetType().GetProperty(q.Name).GetValue(entity, null)).ToArray();
            TEntity obj = DbSet.Find(objArr);
            if (obj != null && obj.GetType().GetProperty("Timestamp") != null)
            {
                entity.GetType().GetProperty("Timestamp").SetValue(entity, obj.GetType().GetProperty("Timestamp").GetValue(obj, null));
                DbContext.Entry(obj).State = EntityState.Detached;
            }
            entityForUpdate = entity;

            return obj != null;
           
        }
        public virtual bool Exists(TEntity entity)
        {
            IEntityType entityType = DbContext.Model.FindEntityType(typeof(TEntity).ToString());
            IKey key = entityType.FindPrimaryKey();
            object[] objArr = key.Properties.Select(q => entity.GetType().GetProperty(q.Name).GetValue(entity, null)).ToArray();
            TEntity obj = DbSet.Find(objArr);
            if (obj != null) DbContext.Entry(obj).State = EntityState.Detached;
            return obj != null; 
        }

        public virtual void InsertOrUpdate(TEntity entity)
        {
            if (ExistsUpdateTimestamp(entity, out var entityForUpdate)) {
                Update(entityForUpdate);
            } 
            else {
                Insert(entity);
            }
        }

        public virtual void InsertOrUpdate(IEnumerable<TEntity> entities) 
            => DbContext.BulkInsertOrUpdate(entities.ToList());

        /// <summary>
        /// Gets the <see cref="List{TEntity}"/> based on a predicate, orderBy delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters"></param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public virtual List<TEntity> GetList(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return query.ToList();
        }

        /// <summary>
        /// Gets the <see cref="List{TEntity}"/> based on a predicate, orderBy delegate and page information. This method default no-tracking query.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <param name="ignoreQueryFilters">Ignore query filters</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An <see cref="List{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public virtual Task<List<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                return orderBy(query).ToListAsync(cancellationToken);
            }

            return query.ToListAsync(cancellationToken);
        }


        /// <summary>
        /// Finds next entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The found entity or null.</returns>
        public virtual TEntity GetNextById(params object[] keyValues)
        {
            TEntity res = DbSet.Find(IncrementKey(keyValues));
            if (res != null)
            {
                return  res;
            }

            //No Result Found. So Order the Entity with key column and select next Entity
            IEntityType entityType = DbContext.Model.FindEntityType(typeof(TEntity).ToString());
            IKey key = entityType.FindPrimaryKey();
            List<String> keyColums = key.Properties.Select(q => q.Name).ToList();
            //var ordByExp = GetOrderBy<TEntity>(keyColums[0],"asc");
            var ordByExp = GetOrderByExpression<TEntity>(keyColums);

            List<TEntity> lstObjs = GetList(null, ordByExp.Compile());

            if (lstObjs != null && lstObjs.Count > 0)
            {
                //Form Where Condition
                Expression<Func<TEntity, bool>> expr = GetWhereConditionExpression<TEntity>(key, DecrementKey(keyValues));
                Func<TEntity, bool> func = expr.Compile();
                Predicate<TEntity> pred = func.Invoke;
                TEntity currObj = lstObjs.Find(pred);

                int curobj = lstObjs.IndexOf(currObj);
                if (curobj != -1)
                {
                    int nxt = curobj + 1;
                    return lstObjs.ElementAtOrDefault(nxt);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Finds next entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The found entity or null.</returns>
        public virtual Task<TEntity> GetNextByIdAsync(params object[] keyValues)
        {
            TEntity res = DbSet.Find(IncrementKey(keyValues));
            if (res != null)
            {
                return Task<TEntity>.Factory.StartNew(() => res);
            }

            //No Result Found. So Order the Entity with key column and select next Entity
            IEntityType entityType = DbContext.Model.FindEntityType(typeof(TEntity).ToString());
            IKey key = entityType.FindPrimaryKey();
            List<String> keyColums = key.Properties.Select(q => q.Name).ToList();
            //var ordByExp = GetOrderBy<TEntity>(keyColums[0],"asc");
            var ordByExp = GetOrderByExpression<TEntity>(keyColums);

            List<TEntity> lstObjs = GetList(null, ordByExp.Compile());

            if (lstObjs != null && lstObjs.Count > 0)
            {
                //Form Where Condition
                Expression<Func<TEntity, bool>> expr = GetWhereConditionExpression<TEntity>(key, DecrementKey(keyValues));
                Func<TEntity, bool> func = expr.Compile();
                Predicate<TEntity> pred = func.Invoke;
                TEntity currObj = lstObjs.Find(pred);

                int curobj = lstObjs.IndexOf(currObj);
                if(curobj != -1)
                {
                    int nxt = curobj + 1;
                    return Task<TEntity>.Factory.StartNew(() => lstObjs.ElementAtOrDefault(nxt));
                }else
                {
                    return Task.FromResult<TEntity>(null);
                }
            }else
            {
                return Task.FromResult<TEntity>(null);
            }
        }

        /// <summary>
        /// Finds previous entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The found entity or null.</returns>
        public virtual TEntity GetPreviousById(params object[] keyValues)
        {
            TEntity res = DbSet.Find(DecrementKey(keyValues));
            if (res != null)
            {
                return  res;
            }

            //No Result Found. So Order the Entity with key column and select next Entity
            IEntityType entityType = DbContext.Model.FindEntityType(typeof(TEntity).ToString());
            IKey key = entityType.FindPrimaryKey();
            List<String> keyColums = key.Properties.Select(q => q.Name).ToList();
            //var ordByExp = GetOrderBy<TEntity>(keyColums[0],"asc");
            var ordByExp = GetOrderByExpression<TEntity>(keyColums);

            List<TEntity> lstObjs = GetList(null, ordByExp.Compile());

            if (lstObjs != null && lstObjs.Count > 0)
            {
                //Form Where Condition
                Expression<Func<TEntity, bool>> expr = GetWhereConditionExpression<TEntity>(key, IncrementKey(keyValues));
                Func<TEntity, bool> func = expr.Compile();
                Predicate<TEntity> pred = func.Invoke;
                TEntity currObj = lstObjs.Find(pred);

                int curobj = lstObjs.IndexOf(currObj);
                if (curobj != -1)
                {
                    int prev = curobj - 1;
                    return  lstObjs.ElementAtOrDefault(prev);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Finds previous entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The found entity or null.</returns>
        public virtual Task<TEntity> GetPreviousByIdAsync(params object[] keyValues)
        {
            TEntity res = DbSet.Find(DecrementKey(keyValues));
            if (res != null)
            {
                return Task<TEntity>.Factory.StartNew(() => res);
            }

            //No Result Found. So Order the Entity with key column and select next Entity
            IEntityType entityType = DbContext.Model.FindEntityType(typeof(TEntity).ToString());
            IKey key = entityType.FindPrimaryKey();
            List<String> keyColums = key.Properties.Select(q => q.Name).ToList();
            //var ordByExp = GetOrderBy<TEntity>(keyColums[0],"asc");
            var ordByExp = GetOrderByExpression<TEntity>(keyColums);

            List<TEntity> lstObjs = GetList(null, ordByExp.Compile());

            if (lstObjs != null && lstObjs.Count > 0)
            {
                //Form Where Condition
                Expression<Func<TEntity, bool>> expr = GetWhereConditionExpression<TEntity>(key, IncrementKey(keyValues));
                Func<TEntity, bool> func = expr.Compile();
                Predicate<TEntity> pred = func.Invoke;
                TEntity currObj = lstObjs.Find(pred);

                int curobj = lstObjs.IndexOf(currObj);
                if (curobj != -1)
                {
                    int prev = curobj - 1;
                    return Task<TEntity>.Factory.StartNew(() => lstObjs.ElementAtOrDefault(prev));
                }
                else
                {
                    return Task.FromResult<TEntity>(null);
                }
            }
            else
            {
                return Task.FromResult<TEntity>(null);
            }
        }

        /// <summary>
        /// Finds the first entity with order by primary key. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <returns>The found entity or null.</returns>
        public virtual TEntity GetFirst()
        {
            //No Result Found. So Order the Entity with key column and select next Entity
            IEntityType entityType = DbContext.Model.FindEntityType(typeof(TEntity).ToString());
            IKey key = entityType.FindPrimaryKey();
            List<String> keyColums = key.Properties.Select(q => q.Name).ToList();
            var ordByExp = GetOrderByExpression<TEntity>(keyColums);

            List<TEntity> lstObjs = GetList(null, ordByExp.Compile());

            if (lstObjs != null && lstObjs.Count > 0)
            {
                    return lstObjs.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Finds the first entity with order by primary key. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <returns>The found entity or null.</returns>
        public virtual Task<TEntity> GetFirstAsync()
        {
            //No Result Found. So Order the Entity with key column and select next Entity
            IEntityType entityType = DbContext.Model.FindEntityType(typeof(TEntity).ToString());
            IKey key = entityType.FindPrimaryKey();
            List<String> keyColums = key.Properties.Select(q => q.Name).ToList();
            var ordByExp = GetOrderByExpression<TEntity>(keyColums);

            List<TEntity> lstObjs = GetList(null, ordByExp.Compile());

            if (lstObjs != null && lstObjs.Count > 0)
            {
                return Task<TEntity>.Factory.StartNew(() => lstObjs.FirstOrDefault());
            }
            else
            {
                return Task.FromResult<TEntity>(null);
            }
        }

        /// <summary>
        /// Finds the Last entity with order by primary key. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <returns>The found entity or null.</returns>
        public virtual TEntity GetLast()
        {
            //No Result Found. So Order the Entity with key column and select next Entity
            IEntityType entityType = DbContext.Model.FindEntityType(typeof(TEntity).ToString());
            IKey key = entityType.FindPrimaryKey();
            List<String> keyColums = key.Properties.Select(q => q.Name).ToList();
            var ordByExp = GetOrderByExpression<TEntity>(keyColums,true);

            List<TEntity> lstObjs = GetList(null, ordByExp.Compile());

            if (lstObjs != null && lstObjs.Count > 0)
            {
                return  lstObjs.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Finds the Last entity with order by primary key. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <returns>The found entity or null.</returns>
        public virtual Task<TEntity> GetLastAsync()
        {
            //No Result Found. So Order the Entity with key column and select next Entity
            IEntityType entityType = DbContext.Model.FindEntityType(typeof(TEntity).ToString());
            IKey key = entityType.FindPrimaryKey();
            List<String> keyColums = key.Properties.Select(q => q.Name).ToList();
            var ordByExp = GetOrderByExpression<TEntity>(keyColums,true);

            List<TEntity> lstObjs = GetList(null, ordByExp.Compile());

            if (lstObjs != null && lstObjs.Count > 0)
            {
                return Task<TEntity>.Factory.StartNew(() => lstObjs.FirstOrDefault());
            }
            else
            {
                return Task.FromResult<TEntity>(null);
            }
        }

        #region Next, Previous Support Methos

        private object[] IncrementKey(object[] id)
        {
            int idx = id.Length -1;
            var val = id[idx];

            if(val.GetType() == typeof(int))
            {
                int iVal = (int)val;
                id[idx] = ++iVal;
            }else if(val.GetType() == typeof(long))
            {
                long lVal = (long)val;
                id[idx] = ++lVal;
            }

            return id;
        }

        private object[] DecrementKey(object[] id)
        {
            int idx = id.Length - 1;
            var val = id[idx];

            if (val.GetType() == typeof(int))
            {
                int iVal = (int)val;
                id[idx] = --iVal;
            }
            else if (val.GetType() == typeof(long))
            {
                long lVal = (long)val;
                id[idx] = --lVal;
            }
            return id;
        }

        private static MemberExpression GetMemberExpression(Expression param, string propertyName)
        {
            if (propertyName.Contains("."))
            {
                int index = propertyName.IndexOf(".");
                var subParam = Expression.Property(param, propertyName.Substring(0, index));
                return GetMemberExpression(subParam, propertyName.Substring(index + 1));
            }
            return Expression.Property(param, propertyName);
        }

        public static Func<IQueryable<T>, IOrderedQueryable<T>> GetOrderBy<T>(string orderColumn, string orderType)
        {
            Type typeQueryable = typeof(IQueryable<T>);
            ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p");
            var outerExpression = Expression.Lambda(argQueryable, argQueryable);
            string[] props = orderColumn.Split('.');
            IQueryable<T> query = new List<T>().AsQueryable();
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");

            Expression expr = arg;
            foreach (string prop in props)
            {
                PropertyInfo pi = type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            LambdaExpression lambda = Expression.Lambda(expr, arg);
            string methodName = orderType == "asc" ? "OrderBy" : "OrderByDescending";

            MethodCallExpression resultExp =
                Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(T), type }, outerExpression.Body, Expression.Quote(lambda));
            var finalLambda = Expression.Lambda(resultExp, argQueryable);
            return (Func<IQueryable<T>, IOrderedQueryable<T>>)finalLambda.Compile();
        }

        public static Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> GetOrderByExpression<T>(IEnumerable<string> lstSelection, bool isDescending = false)
       {
           bool isThenBy = false;
           ParameterExpression inParameter = Expression.Parameter(typeof(T), "s");
           foreach (string propName in lstSelection)
           {
               MemberExpression prop = GetMemberExpression(inParameter, propName); //s.mfrId
               var propertyInfo = (PropertyInfo)prop.Member;
               var lambda = Expression.Lambda(prop, inParameter); // s => s.mfrId
               Type pType = propertyInfo.PropertyType;
               Type[] argumentTypes = new[] { typeof(T),pType };
                if (isThenBy)
               {
                   var thenByMethod = typeof(Queryable).GetMethods()
                   .First(method => method.Name == "ThenBy"
                   && method.GetParameters().Count() == 2)
                  .MakeGenericMethod(argumentTypes);

                   var ThenByDescending = typeof(Queryable).GetMethods()
                   .First(method => method.Name == "ThenByDescending"
                   && method.GetParameters().Count() == 2)
                  .MakeGenericMethod(argumentTypes);
                    if (isDescending)
                    {
                        return query => (IOrderedQueryable<T>)
                            ThenByDescending.Invoke(null, new object[] { query, lambda });
                    }
                    else
                    {
                        return query => (IOrderedQueryable<T>)
                           thenByMethod.Invoke(null, new object[] { query, lambda });
                    }
                }
               else
               {
                    isThenBy = true;
                    var orderByMethod = typeof(Queryable).GetMethods()
                                .First(method => method.Name == "OrderBy"
                                && method.GetParameters().Count() == 2)
                                .MakeGenericMethod(argumentTypes);

                    var orderByDescMethod = typeof(Queryable).GetMethods()
                        .First(method => method.Name == "OrderByDescending"
                            && method.GetParameters().Count() == 2)
                           .MakeGenericMethod(argumentTypes);

                    if (isDescending) {
                        return query => (IOrderedQueryable<T>)
                            orderByDescMethod.Invoke(null, new object[] { query, lambda });
                    }
                    else {
                        return query => (IOrderedQueryable<T>)
                           orderByMethod.Invoke(null, new object[] { query, lambda });
                    }
                }
           }
            return null;
       }

        static MethodInfo LikeMethod = typeof(DbFunctionsExtensions).GetMethod("Like", new Type[] { typeof(DbFunctions), typeof(string), typeof(string) });

        static MethodInfo StartsWithMethod = typeof(String).GetMethod("StartsWith", new Type[] { typeof(String) });

        static MethodInfo ContainsMethod = typeof(String).GetMethod("Contains", new Type[] { typeof(String) });

        static MethodInfo EndsWithMethod = typeof(String).GetMethod("EndsWith", new Type[] { typeof(String) });

        public static Expression<Func<T, bool>> GetWhereConditionExpression<T>(IKey key, params object[] keyValues)
        {
            if (key == null || keyValues == null) return null;
            //Create the expression parameters
            ParameterExpression inParameter = Expression.Parameter(typeof(T));

            Expression whereExp = null;
            int idx = 0;
            foreach (IProperty p in key.Properties)
            {
                string dataPropertyName = p.Name;
                Type dataType = p.PropertyInfo.PropertyType;
                Object propVale = keyValues[idx++];

                if (propVale != null)
                {
                    Expression LHS = GetMemberExpression(inParameter, dataPropertyName); //Expression.Property(inParameter, dataPropertyName);
                    Expression RHS = Expression.Convert(Expression.Constant(propVale), dataType);

                    if (dataType == typeof(DateTime))
                    {
                        //MethodInfo truncTimeMethod = typeof(EF).GetProperty("Functions").GetType().GetMethod("TruncateTime", new Type[] { typeof(DateTime?) });
                        //MethodInfo conMethod = typeof(System.Data.Entity.DbFunctions).GetMethod("TruncateTime", new Type[] { typeof(DateTime?) });
                        //LHS = Expression.Call(conMethod, Expression.Convert(Expression.Property(inParameter, dataPropertyName), typeof(DateTime?)));
                        RHS = Expression.Convert(Expression.Constant(((DateTime)propVale).Date), typeof(DateTime?));
                    }
                    string conOperator = "eq";
                    Expression expr = null;
                    switch (conOperator)
                    {
                        case "<":
                        case "lt":
                            expr = Expression.LessThan(LHS, RHS);
                            break;

                        case ">":
                        case "gt":
                            expr = Expression.GreaterThan(LHS, RHS);
                            break;

                        case "<=":
                        case "le":
                            expr = Expression.LessThanOrEqual(LHS, RHS);
                            break;

                        case ">=":
                        case "ge":
                            expr = Expression.GreaterThanOrEqual(LHS, RHS);
                            break;

                        case "!=":
                        case "<>":
                        case "ne":
                            expr = Expression.NotEqual(LHS, RHS);
                            break;

                        case "IsNull":
                            expr = Expression.Equal(LHS, Expression.Constant(null, dataType));
                            break;

                        case "IsNotNull":
                            expr = Expression.NotEqual(LHS, Expression.Constant(null, dataType));
                            break;

                        case "Like":
                            if (LHS.Type != typeof(string))
                            {
                                LHS = Expression.Convert(Expression.Convert(LHS, typeof(object)), typeof(string));
                            }
                            RHS = Expression.Convert(Expression.Constant(propVale.ToString().Replace(" ", "%") + "%"), dataType);
                            expr = Expression.Call(LikeMethod, Expression.Convert(Expression.Constant(EF.Functions), typeof(DbFunctions)), LHS, RHS);
                            break;

                        case "Contains":
                            if (LHS.Type != typeof(string))
                            {
                                LHS = Expression.Convert(Expression.Convert(LHS, typeof(object)), typeof(string));
                                RHS = Expression.Convert(Expression.Constant("%" + propVale + "%"), dataType);
                                expr = Expression.Call(LikeMethod, Expression.Convert(Expression.Constant(EF.Functions), typeof(DbFunctions)), LHS, RHS);
                            }
                            else
                            {
                                expr = Expression.Call(LHS, ContainsMethod, Expression.Constant(propVale.ToString()));
                            }
                            break;

                        case "StartsWith":
                            if (LHS.Type != typeof(string))
                            {
                                LHS = Expression.Convert(Expression.Convert(LHS, typeof(object)), typeof(string));
                                RHS = Expression.Convert(Expression.Constant(propVale + "%"), dataType);
                                expr = Expression.Call(LikeMethod, Expression.Convert(Expression.Constant(EF.Functions), typeof(DbFunctions)), LHS, RHS);
                            }
                            else
                            {
                                expr = Expression.Call(LHS, StartsWithMethod, Expression.Constant(propVale.ToString()));
                            }
                            break;

                        case "EndsWith":
                            if (LHS.Type != typeof(string))
                            {
                                LHS = Expression.Convert(Expression.Convert(LHS, typeof(object)), typeof(string));
                                RHS = Expression.Convert(Expression.Constant("%" + propVale), dataType);
                                expr = Expression.Call(LikeMethod, Expression.Convert(Expression.Constant(EF.Functions), typeof(DbFunctions)), LHS, RHS);
                            }
                            else
                            {
                                expr = Expression.Call(LHS, EndsWithMethod, Expression.Constant(propVale.ToString()));
                            }
                            break;

                        case "=":
                        case "==":
                        case "eq":
                        default:
                            expr = Expression.Equal(LHS, RHS);
                            break;
                    }

                    if (whereExp == null)
                    {
                        whereExp = expr;
                    }
                    else
                    {
                        String condi = "AND";
                        if (condi != null && condi.Equals("AND", StringComparison.OrdinalIgnoreCase))
                        {
                            whereExp = Expression.AndAlso(whereExp, expr);
                        }
                        else
                        {
                            whereExp = Expression.OrElse(whereExp, expr);
                        }
                    }
                }

            }

            if (whereExp == null)
            {
                whereExp = Expression.Constant(true);
            }
            return Expression.Lambda<Func<T, bool>>(whereExp, inParameter);
        }
        #endregion

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
        public async Task<IList<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity,TResult>> selector,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = DbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            
            if (orderBy != null)
            {
                return await orderBy(query).Select(selector).ToListAsync();
            }
            else
            {
                return await query.Select(selector).ToListAsync();
            }
        }


        /// <summary>
        /// Change entity state for patch method on web api.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// /// <param name="state">The entity state.</param>
        public void ChangeEntityState(TEntity entity, EntityState state) => DbContext.Entry(entity).State = state;

        ValueTask<TEntity> IRepository<TEntity>.FindAsync(params object[] keyValues) => DbSet.FindAsync(keyValues);
        ValueTask<TEntity> IRepository<TEntity>.FindAsync(object[] keyValues, CancellationToken cancellationToken) => DbSet.FindAsync(keyValues, cancellationToken);
    }
}
