namespace PoolIt.Services
{
    using Data;

    public abstract class DataService : BaseService
    {
        protected readonly PoolItDbContext context;

        protected DataService(PoolItDbContext context)
        {
            this.context = context;
        }
    }
}