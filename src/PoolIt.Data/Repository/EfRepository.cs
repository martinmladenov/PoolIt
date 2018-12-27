namespace PoolIt.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.EntityFrameworkCore;

    public class EfRepository<TEntity> : IRepository<TEntity>, IDisposable
        where TEntity : class
    {
        private readonly PoolItDbContext context;

        private readonly DbSet<TEntity> set;

        public EfRepository(PoolItDbContext context)
        {
            this.context = context;
            this.set = this.context.Set<TEntity>();
        }

        public IQueryable<TEntity> All()
            => this.set;

        public Task AddAsync(TEntity entity)
            => this.set.AddAsync(entity);

        public void Remove(TEntity entity)
            => this.set.Remove(entity);

        public void RemoveRange(IEnumerable<TEntity> entity)
            => this.set.RemoveRange(entity);

        public void Update(TEntity entity)
            => this.set.Update(entity);

        public Task<int> SaveChangesAsync()
            => this.context.SaveChangesAsync();

        public void Dispose()
            => this.context.Dispose();
    }
}