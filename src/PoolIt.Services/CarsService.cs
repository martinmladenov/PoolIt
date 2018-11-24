namespace PoolIt.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
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

        public async Task<bool> CreateAsync(CarServiceModel model)
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

            var owner = await this.context.Users.SingleOrDefaultAsync(u => u.UserName == model.Owner.UserName);

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

        public async Task<IEnumerable<CarServiceModel>> GetAllForUserAsync(string userName)
        {
            if (userName == null)
            {
                return null;
            }

            var user = await this.context.Users.SingleOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return null;
            }

            var cars = await this.context.Cars
                .Where(c => c.OwnerId == user.Id)
                .ProjectTo<CarServiceModel>()
                .ToArrayAsync();

            return cars;
        }

        public async Task<CarServiceModel> GetAsync(string id)
        {
            if (id == null)
            {
                return null;
            }

            var car = await this.context.Cars
                .ProjectTo<CarServiceModel>()
                .SingleOrDefaultAsync(u => u.Id == id);

            return car;
        }
    }
}