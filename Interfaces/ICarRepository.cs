using CarDealer.Models;
namespace CarDealer.Interfaces{
    public interface ICarRepository{
        Task<ICollection<CarItem>>  GetCarsAsync();
        Task<CarItem> GetCarAsync(int id);
        Task<bool> CreateCarAsync(CarItem car);
        Task<bool> UpdateCarAsync(int id, CarItem car);
        Task<bool> DeleteCarAsync(int carId);
        Task<bool> SaveAsync();
        Task<bool> CarExistsAsync(int carId);

    }
}