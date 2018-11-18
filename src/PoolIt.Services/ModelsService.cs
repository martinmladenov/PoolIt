namespace PoolIt.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;

    public class ModelsService : DataService, IModelsService
    {
        public ModelsService(PoolItDbContext context) : base(context)
        {
        }

        public async Task<CarModelServiceModel> Get(string id)
        {
            if (id == null)
            {
                return null;
            }

            var model = await this.context.CarModels
                .FirstOrDefaultAsync(m => m.Id == id);

            if (model == null)
            {
                return null;
            }

            return Mapper.Map<CarModelServiceModel>(model);
        }

        public async Task<bool> Create(CarModelServiceModel serviceModel)
        {
            if (!this.IsEntityStateValid(serviceModel))
            {
                return false;
            }

            var model = Mapper.Map<CarModel>(serviceModel);

            await this.context.CarModels.AddAsync(model);

            await this.context.SaveChangesAsync();

            return true;
        }
    }
}