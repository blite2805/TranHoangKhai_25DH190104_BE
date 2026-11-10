using PagedList;
using System.Collections.Generic;

namespace _25DH190104_TranHoangKhai_BE.Models
{
    // ViewModel cho trang chủ (Home/Index): sản phẩm nổi bật + sản phẩm mới có phân trang PagedList
    public class HomeProductVM
    {
        public List<Product> FeaturedProducts { get; set; }
        public IPagedList<Product> NewProducts { get; set; }
    }
}
