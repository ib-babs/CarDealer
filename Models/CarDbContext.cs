using Microsoft.EntityFrameworkCore;
namespace CarDealer.Models
{
    public class CarDbContext(DbContextOptions<CarDbContext> opts): DbContext(opts)
    {
       public DbSet<CarItem> Cars => Set<CarItem>();
    }
}
