//-----------------------------------------------------------------------
// <copyright file="IRepositoryFactory.cs" company="Arch team">
// Copyright (c) Arch team. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Arch.EntityFrameworkCore.UnitOfWork
{
    /// <summary>
    /// Defines the interfaces for <see cref="IRepository{TEntity}"/> interfaces.
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Gets the specified repository for the <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="hasCustomRepository"><c>True</c> if providing custom repositry</param>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref="IRepository{TEntity}"/> interface.</returns>
        IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class;
    }
}