namespace UnitOfWork
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class;
    }
}