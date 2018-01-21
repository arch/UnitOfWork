using System;
using System.Collections.Generic;


namespace Microsoft.EntityFrameworkCore.UnitOfWork
{
    /// <summary>
    /// This interface is used to inject a bunch of custom Repository in the uow
    /// </summary>
    public interface IUnitOfWorkRepositoryProvider
    {
        /// <summary>
        /// Dictionary of custom repositories
        /// </summary>
        Dictionary<Type, object> RepositoriesDictionary
        {
            get;
        }
    }
}
