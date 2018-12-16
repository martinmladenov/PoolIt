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

    public class CarsService : BaseService, ICarsService
    {
        private readonly IRepository<Car> carsRepository;
        private readonly IRepository<CarModel> carModelsRepository;
        private readonly IRepository<PoolItUser> usersRepository;

        public CarsService(IRepository<Car> carsRepository, IRepository<CarModel> carModelsRepository,
            IRepository<PoolItUser> usersRepository)
        {
            this.carsRepository = carsRepository;
            this.carModelsRepository = carModelsRepository;
            this.usersRepository = usersRepository;
        }

        public async Task<bool> CreateAsync(CarServiceModel model)
        {
            if (!this.IsEntityStateValid(model))
            {
                return false;
            }

            if (!await this.carModelsRepository.All()
                .AnyAsync(m => m.Id == model.ModelId))
            {
                return false;
            }

            var owner = await this.usersRepository.All().SingleOrDefaultAsync(u => u.UserName == model.Owner.UserName);

            if (owner == null)
            {
                return false;
            }

            var car = Mapper.Map<Car>(model);

            car.Owner = owner;

            await this.carsRepository.AddAsync(car);

            await this.carsRepository.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CarServiceModel>> GetAllForUserAsync(string userName)
        {
            if (userName == null)
            {
                return null;
            }

            var user = await this.usersRepository.All().SingleOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return null;
            }

            var cars = await this.carsRepository.All()
                .Where(c => c.OwnerId == user.Id)
                .ProjectTo<CarServiceModel>()
                .ToArrayAsync();

            return cars;
        }

        public async Task<IEnumerable<CarServiceModel>> GetAllAsync()
        {
            var cars = await this.carsRepository.All()
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

            var car = await this.carsRepository.All()
                .ProjectTo<CarServiceModel>()
                .SingleOrDefaultAsync(u => u.Id == id);

            return car;
        }

        public bool IsUserOwner(CarServiceModel carServiceModel, string userName)
            => userName != null &&
               carServiceModel.Owner.UserName == userName;

        public async Task<bool> UpdateAsync(CarServiceModel model)
        {
            if (!this.IsEntityStateValid(model) || model.Id == null)
            {
                return false;
            }

            var car = await this.carsRepository.All().SingleOrDefaultAsync(c => c.Id == model.Id);

            if (car == null)
            {
                return false;
            }

            car.Colour = model.Colour;
            car.Details = model.Details;

            this.carsRepository.Update(car);
            await this.carsRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (id == null)
            {
                return false;
            }

            var car = await this.carsRepository.All()
                .Include(r => r.Rides)
                .SingleOrDefaultAsync(r => r.Id == id);

            if (car == null || car.Rides.Any())
            {
                return false;
            }

            this.carsRepository.Remove(car);

            await this.carsRepository.SaveChangesAsync();

            return true;
        }
    }
}