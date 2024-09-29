using System.ComponentModel.DataAnnotations.Schema;

namespace CarDealer.Models
{
    public class CarItem
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; } = String.Empty;
        public string Color { get; set; } = String.Empty;
        public string Model { get; set; } = String.Empty;
        public int WheelQuantity { get; set; }
        public int Year { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; }
        public string Features { get; set; } = String.Empty;
        public string? ImagePath { get; set; }
    }
}
