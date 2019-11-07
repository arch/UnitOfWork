using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace UnitOfWork
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> FromSql(string sql, params object[] parameters);
        TEntity Find(params object[] keyValues);
        Task<TEntity> FindAsync(params object[] keyValues);
        Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken);
        int Count(Expression<Func<TEntity, bool>> predicate = null);
        void Insert(TEntity entity);
        void Insert(params TEntity[] entities);
        void Insert(IEnumerable<TEntity> entities);
        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task InsertAsync(params TEntity[] entities);
        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void Update(params TEntity[] entities);
        void Update(IEnumerable<TEntity> entities);
        void Delete(object id);
        void Delete(TEntity entity);
        void Delete(params TEntity[] entities);
        void Delete(IEnumerable<TEntity> entities);

        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null);

        Task<IEnumerable<TEntity>> GetListAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null);

        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null);

        IEnumerable<TEntity> GetList(Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null);

        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null);

        TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null);

        Task<IEnumerable<TProjectedEntity>> GetProjectedListAsync<TProjectedEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TProjectedEntity> projectedBy,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null);

        Task<IEnumerable<TProjectedEntity>> GetProjectedListAsync<TProjectedEntity>(
            Func<TEntity, TProjectedEntity> projectedBy,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null);

        IEnumerable<TProjectedEntity> GetProjectedList<TProjectedEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TProjectedEntity> projectedBy,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null);

        IEnumerable<TProjectedEntity> GetProjectedList<TProjectedEntity>(Func<TEntity, TProjectedEntity> projectedBy,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> extendBy = null);
    }
}