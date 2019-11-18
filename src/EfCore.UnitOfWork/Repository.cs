// Copyright (c) Arch team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EfCore.UnitOfWork
{
    public sealed class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null) =>
            await GetListInternal(predicate, extendBy).ToListAsync();           
        
        public async Task<IEnumerable<TProjectedEntity>> GetProjectedListAsync<TProjectedEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TProjectedEntity> projectedBy,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null) =>
            await GetListProjectedByInternal(predicate, projectedBy, extendBy).ToListAsync();        
        
        public async Task<IEnumerable<TEntity>> GetListAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null) =>
            await GetListInternal(p => true, extendBy).ToListAsync();        
        
        public async Task<IEnumerable<TProjectedEntity>> GetProjectedListAsync<TProjectedEntity>(
            Func<TEntity, TProjectedEntity> projectedBy,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null) =>
            await GetListProjectedByInternal(p => true, projectedBy, extendBy).ToListAsync();
        
        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null) =>
            GetListInternal(predicate, extendBy).ToList();        
        
        public IEnumerable<TProjectedEntity> GetProjectedList<TProjectedEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TProjectedEntity> projectedBy,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null) =>
            GetListProjectedByInternal(predicate, projectedBy, extendBy).ToList();
        
        public IEnumerable<TEntity> GetList(Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null) =>
            GetListInternal(p => true, extendBy).ToList();        
        
        public IEnumerable<TProjectedEntity> GetProjectedList<TProjectedEntity>(Func<TEntity, TProjectedEntity> projectedBy,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null) =>
            GetListProjectedByInternal(p => true, projectedBy, extendBy).ToList();

        public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null) =>
            await GetListInternal(predicate, extendBy).FirstOrDefaultAsync();        
        
        public TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null) =>
            GetListInternal(predicate, extendBy).FirstOrDefault();

        private IQueryable<TEntity> GetListInternal(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extend = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var queryable = _dbSet.Where(predicate);
            if (extend != null)
                queryable = extend(queryable);

            return queryable;
        } 
        private IQueryable<TProjectedEntity> GetListProjectedByInternal<TProjectedEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TProjectedEntity> projectedBy,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extend = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var queryable = GetListInternal(predicate, extend);
            return queryable.Select(i => projectedBy(i));
        }         
        
        public IQueryable<TEntity> FromSql(string sql, params object[] parameters) => _dbSet.FromSql(sql, parameters);

        public TEntity Find(params object[] keyValues) => _dbSet.Find(keyValues);

        public Task<TEntity> FindAsync(params object[] keyValues) => _dbSet.FindAsync(keyValues);

        public Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken) => _dbSet.FindAsync(keyValues, cancellationToken);

        public int Count(Expression<Func<TEntity, bool>> predicate = null) => predicate == null ? _dbSet.Count() : _dbSet.Count(predicate);

        public void Insert(TEntity entity) => _dbSet.Add(entity);

        public void Insert(params TEntity[] entities) => _dbSet.AddRange(entities);

        public void Insert(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);

        public Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default) => _dbSet.AddAsync(entity, cancellationToken);

        public Task InsertAsync(params TEntity[] entities) => _dbSet.AddRangeAsync(entities);

        public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => _dbSet.AddRangeAsync(entities, cancellationToken);

        public void Update(TEntity entity) => _dbSet.Update(entity);

        public void Update(params TEntity[] entities) => _dbSet.UpdateRange(entities);

        public void Update(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

        public void Delete(TEntity entity) => _dbSet.Remove(entity);

        public void Delete(object id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        public void Delete(params TEntity[] entities) => _dbSet.RemoveRange(entities);

        public void Delete(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);
    }
}
