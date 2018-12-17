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

    public class ModelsService : BaseService, IModelsService
    {
        private readonly IRepository<CarModel> carModelsRepository;

        public ModelsService(IRepository<CarModel> carModelsRepository)
        {
            this.carModelsRepository = carModelsRepository;
        }

        public async Task<bool> CreateAsync(CarModelServiceModel serviceModel)
        {
            if (!this.IsEntityStateValid(serviceModel))
            {
                return false;
            }

            var model = Mapper.Map<CarModel>(serviceModel);

            await this.carModelsRepository.AddAsync(model);

            await this.carModelsRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(CarModelServiceModel serviceModel)
            => await this.carModelsRepository.All().AnyAsync(m =>
                m.ManufacturerId == serviceModel.ManufacturerId &&
                string.Equals(m.Model, serviceModel.Model, StringComparison.InvariantCultureIgnoreCase));

        public async Task<IEnumerable<CarModelServiceModel>> GetAllByManufacturerAsync(string manufacturerId)
        {
            if (manufacturerId == null)
            {
                return null;
            }

            var models = await this.carModelsRepository.All()
                .Where(m => m.ManufacturerId == manufacturerId)
                .ProjectTo<CarModelServiceModel>()
                .ToArrayAsync();

            return models;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (id == null)
            {
                return false;
            }

            var carManufacturer = await this.carModelsRepository.All()
                .Include(r => r.Cars)
                .SingleOrDefaultAsync(r => r.Id == id);

            if (carManufacturer == null || carManufacturer.Cars.Any())
            {
                return false;
            }

            this.carModelsRepository.Remove(carManufacturer);

            await this.carModelsRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(CarModelServiceModel model)
        {
            if (!this.IsEntityStateValid(model) || model.Id == null)
            {
                return false;
            }

            var carManufacturer =
                await this.carModelsRepository.All().SingleOrDefaultAsync(c => c.Id == model.Id);

            if (carManufacturer == null)
            {
                return false;
            }

            carManufacturer.Model = model.Model;

            this.carModelsRepository.Update(carManufacturer);
            await this.carModelsRepository.SaveChangesAsync();

            return true;
        }
    }
}