namespace CarDealer.Models
{
    public class CarListItem
    {
        public required CarItem CarItem { get; set; } 
        public required CarImageUploader Image { get; set; }
    }
}
