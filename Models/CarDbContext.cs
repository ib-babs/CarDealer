using Microsoft.EntityFrameworkCore;
namespace CarDealer.Models
{
    public class CarDbContext(DbContextOptions<CarDbContext> opts): DbContext(opts)
    {
       public DbSet<CarItem> CarItems => Set<CarItem>();
    }
}
