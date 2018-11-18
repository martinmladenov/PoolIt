namespace PoolIt.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Contracts;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using PoolIt.Models;

    public class CarsService : DataService, ICarsService
    {
        public CarsService(PoolItDbContext context) : base(context)
        {
        }

        public async Task<bool> Create(CarServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            if (!await this.context.CarModels
                .AnyAsync(m => m.Id == model.ModelId))
            {
                return false;
            }

            var owner = await this.context.Users.FirstOrDefaultAsync(u => u.UserName == model.Owner.UserName);

            if (owner == null)
            {
                return false;
            }

            var car = Mapper.Map<Car>(model);

            car.Owner = owner;

            await this.context.Cars.AddAsync(car);

            await this.context.SaveChangesAsync();

            return true;
        }
    }
}