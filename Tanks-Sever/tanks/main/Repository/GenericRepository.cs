using Microsoft.EntityFrameworkCore;
using Repository.Interfaces.Repository;

namespace Repository
{
    public class GenericRepository : IGenericRepository
    {
        private readonly GameDbContext _context;

        public GenericRepository(GameDbContext context)
        {
            _context = context;
        }

        // Получить одну сущность по ID
        public TEntity Single<TEntity>(long id) where TEntity : class
        {
            return _context.Set<TEntity>().Find(id);
        }

        // Получить одну сущность по Никнейму
        public TEntity SingleByNickname<TEntity>(string nickname) where TEntity : class
        {
            return _context.Set<TEntity>().SingleOrDefault(e => EF.Property<string>(e, "Nickname") == nickname);
        }

        // Получить сущность по никнейму (UserId) *для гаража*
        public TEntity SingleByUserId<TEntity>(string userId) where TEntity : class
        {
            return _context.Set<TEntity>().SingleOrDefault(e => EF.Property<string>(e, "UserId") == userId);
        }

        // Получить все сущности из таблицы
        public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>().AsNoTracking().ToList();
        }

        // Метод Select<TEntity> для выполнения запросов
        public IQueryable<TEntity> Select<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        // Добавить сущность
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Add(entity);
            SaveChanges();
        }

        // Обновить сущность
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Update(entity);
            SaveChanges();
        }

        // Удалить сущность
        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Remove(entity);
            SaveChanges();
        }

        // Сохранить изменения в базе данных
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
