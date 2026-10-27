using PagedList;
using System.Collections.Generic;

namespace _25DH190104_TranHoangKhai_BE.Models.ViewModel
{
    public class ProductSearchVM
    {
        public IPagedList<Product> Products { get; set; }

        public string SearchTerm { get; set; }

        public decimal? MinPrice { get; set; }  

        public decimal? MaxPrice { get; set; }

        public string SortOrder { get; set; }
    }
}