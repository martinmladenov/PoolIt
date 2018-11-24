namespace PoolIt.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;

    public class ManufacturersService : DataService, IManufacturersService
    {
        public ManufacturersService(PoolItDbContext context) : base(context)
        {
        }

        public async Task<CarManufacturerServiceModel> GetAsync(string name)
        {
            if (name == null)
            {
                return null;
            }
            
            var manufacturer = await this.context.CarManufacturers
                .SingleOrDefaultAsync(m => m.Name == name);

            if (manufacturer == null)
            {
                return null;
            }

            return Mapper.Map<CarManufacturerServiceModel>(manufacturer);
        }

        public async Task<IEnumerable<CarManufacturerServiceModel>> GetAllAsync()
        {
            var manufacturers = await this.context.CarManufacturers
                .ProjectTo<CarManufacturerServiceModel>()
                .ToArrayAsync();

            return manufacturers;
        }
    }
}