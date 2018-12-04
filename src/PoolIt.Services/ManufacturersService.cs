namespace PoolIt.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Contracts;
    using Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;

    public class ManufacturersService : BaseService, IManufacturersService
    {
        private readonly IRepository<CarManufacturer> carManufacturersRepository;

        public ManufacturersService(IRepository<CarManufacturer> carManufacturersRepository)
        {
            this.carManufacturersRepository = carManufacturersRepository;
        }

        public async Task<CarManufacturerServiceModel> GetAsync(string name)
        {
            if (name == null)
            {
                return null;
            }

            var manufacturer = await this.carManufacturersRepository.All()
                .SingleOrDefaultAsync(m => m.Name == name);

            if (manufacturer == null)
            {
                return null;
            }

            return Mapper.Map<CarManufacturerServiceModel>(manufacturer);
        }

        public async Task<IEnumerable<CarManufacturerServiceModel>> GetAllAsync()
        {
            var manufacturers = await this.carManufacturersRepository.All()
                .ProjectTo<CarManufacturerServiceModel>()
                .ToArrayAsync();

            return manufacturers;
        }
    }
}