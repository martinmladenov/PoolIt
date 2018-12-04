namespace PoolIt.Data.Common
{
    using System.Linq;
    using System.Threading.Tasks;

    public interface IRepository<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> All();
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
        Task<int> SaveChangesAsync();
    }
}