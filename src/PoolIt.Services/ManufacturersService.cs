namespace PoolIt.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
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

        public async Task<CarManufacturerServiceModel> Get(string name)
        {
            if (name == null)
            {
                return null;
            }
            
            var manufacturer = await this.context.CarManufacturers
                .FirstOrDefaultAsync(m => m.Name == name);

            if (manufacturer == null)
            {
                return null;
            }

            return Mapper.Map<CarManufacturerServiceModel>(manufacturer);
        }

        public async Task<bool> Create(CarManufacturerServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            if (await this.context.CarManufacturers
                .AnyAsync(m => m.Name != model.Name))
            {
                return false;
            }

            var manufacturer = Mapper.Map<CarManufacturer>(model);

            await this.context.CarManufacturers.AddAsync(manufacturer);

            await this.context.SaveChangesAsync();

            return true;
        }
    }
}