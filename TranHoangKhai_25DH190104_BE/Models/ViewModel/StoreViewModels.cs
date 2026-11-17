using System.Collections.Generic;
using PagedList;
using TranHoangKhai_25DH190104_BE.Models;

namespace TranHoangKhai_25DH190104_BE.Models.ViewModel
{
    // Trang Home khach hang: san pham noi bat + san pham moi (phan trang)
    public class HomeProductVM
    {
        public string SearchTerm { get; set; }
        public List<Product> FeaturedProducts { get; set; }   // top 10 ban chay
        public IPagedList<Product> NewProducts { get; set; }  // 20 sp moi, 6/trang
    }

    // Trang chi tiet san pham: sp hien tai + sp tuong tu + top deal (phan trang)
    public class ProductDetailVM
    {
        public Product Product { get; set; }
        public int Quantity { get; set; } = 1;
        public List<Product> RelatedProducts { get; set; }    // 8 sp cung danh muc
        public IPagedList<Product> TopProducts { get; set; }  // ban chay nhat cung danh muc
    }

    // Tim kiem nang cao + sap xep + phan trang (Admin Products va Customer)
    public class ProductSearchVM
    {
        public string Keyword { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortOrder { get; set; } // name_asc, name_desc, price_asc, price_desc
        public int? Page { get; set; }
        public int PageSize { get; set; } = 6;
        public IPagedList<Product> Products { get; set; }
    }

    // Menu ca nhan tren header: trang thai dang nhap + so luong gio hang
    public class PersonalMenuVM
    {
        public bool IsLoggedIn { get; set; }
        public string Username { get; set; }
        public int CartCount { get; set; }
    }

    // Form thanh toan
    public class CheckoutVM
    {
        public string ShippingAddress { get; set; }
        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Total { get; set; }
    }
}
