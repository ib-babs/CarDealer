using CarDealer.Models;

namespace CarDealer.Service
{
    public class PageListView
    {
        public IEnumerable<CarItem> Cars { get; set; } = [];
        public PageItem? PageItem { get; set; }
    }
}
