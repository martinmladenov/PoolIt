namespace PoolIt.Services
{
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
    }
}