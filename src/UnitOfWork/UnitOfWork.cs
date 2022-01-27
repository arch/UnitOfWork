// Copyright (c) Arch team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Arch.EntityFrameworkCore.UnitOfWork
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IUnitOfWork"/> and <see cref="IUnitOfWork{TContext}"/> interface.
    /// </summary>
    /// <typeparam name="TContext">The type of the db context.</typeparam>
    public class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork<TContext> where TContext : DbContext
    {
        private bool _disposed;
        private Dictionary<Type, object> _repositories;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork{TContext}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UnitOfWork(TContext context) => DbContext = context ?? throw new ArgumentNullException(nameof(context));

        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <returns>The instance of type <typeparamref name="TContext"/>.</returns>
        public TContext DbContext { get; }

        /// <summary>
        /// Changes the database name. This require the databases in the same machine. NOTE: This only work for MySQL right now.
        /// </summary>
        /// <param name="database">The database name.</param>
        /// <remarks>
        /// This only been used for supporting multiple databases in the same model. This require the databases in the same machine.
        /// </remarks>
        public void ChangeDatabase(string database)
        {
            var connection = DbContext.Database.GetDbConnection();
            if (connection.State.HasFlag(ConnectionState.Open))
            {
                connection.ChangeDatabase(database);
            }
            else
            {
                var connectionString = Regex.Replace(connection.ConnectionString.Replace(" ", ""), @"(?<=[Dd]atabase=)\w+(?=;)", database, RegexOptions.Singleline);
                connection.ConnectionString = connectionString;
            }

            // Following code only working for mysql.
            var items = DbContext.Model.GetEntityTypes();
            foreach (var item in items)
            {
                if (item is IConventionEntityType entityType)
                {
                    entityType.SetSchema(database);
                }
            }
        }

        /// <summary>
        /// Gets the specified repository for the <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="hasCustomRepository"><c>True</c> if providing custom repository</param>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref="IRepository{TEntity}"/> interface.</returns>
        public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<Type, object>();
            }

            // what's the best way to support custom repository?
            if (hasCustomRepository)
            {
                var customRepo = DbContext.GetService<IRepository<TEntity>>();
                if (customRepo != null)
                {
                    return customRepo;
                }
            }

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<TEntity>(DbContext);
            }

            return (IRepository<TEntity>)_repositories[type];
        }

        /// <summary>
        /// Executes the specified raw SQL command.
        /// </summary>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The number of state entities written to database.</returns>
        public int ExecuteSqlCommand(string sql, params object[] parameters) => DbContext.Database.ExecuteSqlRaw(sql, parameters);

        /// <summary>
        /// Executes the specified raw SQL command.
        /// </summary>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The DataTable.</returns>
        public DataTable ExecuteDtSqlCommand(string sql, params object[] parameters)
        {
           SqlConnection conn = (SqlConnection) DbContext.Database.GetDbConnection();
           SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandTimeout = 0;

            if(parameters != null && parameters.Count() > 0)
            {
                foreach(object obj in parameters)
                {
                    cmd.Parameters.Add(obj);
                }
            }

            conn.Open();
            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            da.Fill(ds);
            da.Fill(dataTable);
            da.Dispose();
            conn.Close();
            return dataTable;
        }


        /// <summary>
        /// Uses raw SQL queries to fetch the specified <typeparamref name="TEntity" /> data.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An <see cref="IQueryable{T}" /> that contains elements that satisfy the condition specified by raw SQL.</returns>
        public IQueryable<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class => DbContext.Set<TEntity>().FromSqlRaw(sql, parameters);

        /// <summary>
        /// Starts Databaselevel Transaction
        /// </summary>
        /// <param name="isolation">The IsolationLevel</param>
        /// <returns>Transaction Context</returns>
        public IDbContextTransaction BeginTransaction(System.Data.IsolationLevel isolation = System.Data.IsolationLevel.ReadCommitted) => DbContext.Database.BeginTransaction(isolation);

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public int SaveChanges(bool ensureAutoHistory = false)
        {
            if (ensureAutoHistory)
            {
                DbContext.EnsureAutoHistory();
            }

            return DbContext.SaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        public async Task<int> SaveChangesAsync(bool ensureAutoHistory = false)
        {
            if (ensureAutoHistory)
            {
                DbContext.EnsureAutoHistory();
            }

            return await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Saves all changes made in this context to the database with distributed transaction.
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
        /// <param name="unitOfWorks">An optional <see cref="IUnitOfWork"/> array.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        public async Task<int> SaveChangesAsync(bool ensureAutoHistory = false, params IUnitOfWork[] unitOfWorks)
        {
            using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var count = 0;
            foreach (var unitOfWork in unitOfWorks)
            {
                count += await unitOfWork.SaveChangesAsync(ensureAutoHistory).ConfigureAwait(false);
            }

            count += await SaveChangesAsync(ensureAutoHistory);

            ts.Complete();

            return count;
        }


        /// <summary>
        /// Saves all changes made in this context to the database with distributed transaction.
        /// </summary>
        /// <param name="transaction">The transaction to use</param>
        /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
        /// <param name="unitOfWorks">An optional <see cref="IUnitOfWork"/> array.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        public async Task<int> SaveChangesAsync(Transaction transaction, bool ensureAutoHistory = false,  params IUnitOfWork[] unitOfWorks)
        {
            using var ts = new TransactionScope(transaction);
            var count = 0;
            foreach (var unitOfWork in unitOfWorks)
            {
                count += await unitOfWork.SaveChangesAsync(ensureAutoHistory);
            }

            count += await SaveChangesAsync(ensureAutoHistory);

            ts.Complete();

            return count;
        }
        
        public void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback) => DbContext.ChangeTracker.TrackGraph(rootEntity, callback);

        IDbContextTransaction IUnitOfWork.BeginTransaction(System.Data.IsolationLevel isolation) => DbContext.Database.BeginTransaction(isolation);


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">The disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // clear repositories
                    _repositories?.Clear();

                    // dispose the db context.
                    DbContext.Dispose();
                }
            }

            _disposed = true;
        }
        
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">The disposing.</param>
        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // clear repositories
                    _repositories?.Clear();

                    // dispose the db context.
                    await DbContext.DisposeAsync();
                }
            }

            _disposed = true;
        }
    }
}
