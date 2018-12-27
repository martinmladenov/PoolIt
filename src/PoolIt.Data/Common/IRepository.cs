namespace PoolIt.Data.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IRepository<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> All();
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entity);
        void Update(TEntity entity);
        Task<int> SaveChangesAsync();
    }
}