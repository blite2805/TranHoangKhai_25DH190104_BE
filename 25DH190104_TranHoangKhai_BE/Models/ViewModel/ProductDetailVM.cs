using System.Collections.Generic;

namespace _25DH190104_TranHoangKhai_BE.Models
{
    // ViewModel cho trang chi tiết sản phẩm (Home/ProductDetail)
    public class ProductDetailVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public List<Product> TopProducts { get; set; }
    }
}
