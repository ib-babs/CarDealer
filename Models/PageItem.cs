namespace CarDealer.Models
{
    public class PageItem
    {
        public int CurrentPage { get; set; }
        public int ItemPerPage { get; set; }
        public int TotalItem { get; set; }

        public decimal TotalPage => Math.Ceiling((decimal)TotalItem / ItemPerPage);
    }
}
