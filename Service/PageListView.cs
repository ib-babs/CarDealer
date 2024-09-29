using CarDealer.Models;

namespace CarDealer.Service
{
    public class PageListView
    {
        public IEnumerable<CarItem> CarItems { get; set; } = [];
        public PageItem? PageItem { get; set; }
    }
}
