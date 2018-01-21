using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore.UnitOfWork
{
    /// <summary>
    /// Interface to inject DbContext into the custom repository
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface ICustomRepository<TContext>
        where TContext : DbContext
    {
    }
}
