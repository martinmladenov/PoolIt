namespace PoolIt.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public async Task<CarManufacturerServiceModel> GetByNameAsync(string name)
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

        public async Task<bool> CreateAsync(CarManufacturerServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            var carManufacturer = Mapper.Map<CarManufacturer>(model);

            await this.carManufacturersRepository.AddAsync(carManufacturer);

            await this.carManufacturersRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (id == null)
            {
                return false;
            }

            var carManufacturer = await this.carManufacturersRepository.All()
                .Include(r => r.Models)
                .SingleOrDefaultAsync(r => r.Id == id);

            if (carManufacturer == null || carManufacturer.Models.Any())
            {
                return false;
            }

            this.carManufacturersRepository.Remove(carManufacturer);

            await this.carManufacturersRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(CarManufacturerServiceModel model)
        {
            if (!this.IsEntityStateValid(model) || model.Id == null)
            {
                return false;
            }

            var carManufacturer =
                await this.carManufacturersRepository.All().SingleOrDefaultAsync(c => c.Id == model.Id);

            if (carManufacturer == null)
            {
                return false;
            }

            carManufacturer.Name = model.Name;

            this.carManufacturersRepository.Update(carManufacturer);
            await this.carManufacturersRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(CarManufacturerServiceModel model)
            => await this.carManufacturersRepository.All()
                .AnyAsync(r => string.Equals(r.Name, model.Name, StringComparison.InvariantCultureIgnoreCase));
    }
}