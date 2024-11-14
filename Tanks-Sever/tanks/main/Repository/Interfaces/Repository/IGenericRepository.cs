namespace Repository.Interfaces.Repository
{
    public interface IGenericRepository
    {
        TEntity Single<TEntity>(long id) where TEntity : class;
        TEntity SingleByNickname<TEntity>(string nickname) where TEntity : class;
        TEntity SingleByUserId<TEntity>(string nickname) where TEntity : class;
        IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class;
        IQueryable<TEntity> Select<TEntity>() where TEntity : class;
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        void Delete<TEntity>(TEntity entity) where TEntity : class;
        void SaveChanges();
    }
}
