using Microsoft.EntityFrameworkCore;
using CarDealer.Interfaces;
using CarDealer.Models;
namespace CarDealer.Repositories
{
    public class CarRepository(CarDbContext context) : ICarRepository
    {
        private readonly CarDbContext _context = context;

        public async Task<bool> CarExistsAsync(int carId) => await _context.Cars.AnyAsync(car => car.Id == carId);

        public async Task<bool> CreateCarAsync(CarItem car)
        {
            await _context.Cars.AddAsync(car);
            return await SaveAsync();
        }

        public async Task<bool> DeleteCarAsync(int carId)
        {
            var car = await GetCarAsync(carId);
            _context.Cars.Remove(car);
            return await SaveAsync();
        }

        public async Task<CarItem> GetCarAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        public async Task<ICollection<CarItem>> GetCarsAsync()
        {
            return await _context.Cars.OrderBy(car => car.Id).ToListAsync();
        }

        public async Task<bool> UpdateCarAsync(int carId, CarItem car)
        {
            var existingCar = await GetCarAsync(carId);
            car.Id = carId;
            car.ImagePath = existingCar.ImagePath;
            _context.Entry(existingCar).CurrentValues.SetValues(car);
            return await SaveAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
    }
}