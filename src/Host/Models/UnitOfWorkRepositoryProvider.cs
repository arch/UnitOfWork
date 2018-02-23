using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.UnitOfWork;

namespace Host.Models
{
    /// <summary>
    /// Provider for my custom repositories
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class UnitOfWorkRepositoryProvider<TContext> : IUnitOfWorkRepositoryProvider
           where TContext : DbContext
    {
        public Dictionary<Type, object> RepositoriesDictionary
        {
            get;
        }

        #region Constructor
        /// <summary>
        /// Inject all the custom repositories needed for the application
        /// </summary>
        /// <param name="blogRepository"></param>
        public UnitOfWorkRepositoryProvider(IBlogRepository blogRepository)
        {
            RepositoriesDictionary = new Dictionary<Type, object>();

            RepositoriesDictionary.Add(typeof(Blog), blogRepository);
        }
        #endregion
    }
}
