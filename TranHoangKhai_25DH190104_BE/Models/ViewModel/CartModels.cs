using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TranHoangKhai_25DH190104_BE.Models;

namespace TranHoangKhai_25DH190104_BE.Models.ViewModel
{
    // Mot mat hang trong gio
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; } // dung de nhom gio hang nang cao
        public decimal Total => UnitPrice * Quantity;
    }

    // Gio hang: danh sach mat hang + nhom theo danh muc + sp tuong tu
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public List<IGrouping<string, CartItem>> GroupedItems =>
            Items.GroupBy(i => i.Category).ToList();
        public IPagedList<Product> SimilarProducts { get; set; }
        public decimal Total => Items.Sum(i => i.Total);
        public int Count => Items.Sum(i => i.Quantity);
    }

    // Dich vu gio hang luu trong Session
    public static class CartService
    {
        private const string Key = "Cart";
        public static Cart GetCart()
        {
            var cart = HttpContext.Current.Session[Key] as Cart;
            if (cart == null) { cart = new Cart(); HttpContext.Current.Session[Key] = cart; }
            return cart;
        }
        public static void AddItem(Product p, int qty)
        {
            var cart = GetCart();
            var it = cart.Items.FirstOrDefault(i => i.ProductID == p.ProductID);
            if (it == null)
                cart.Items.Add(new CartItem {
                    ProductID = p.ProductID, ProductName = p.ProductName,
                    ProductImage = p.ProductImage, UnitPrice = p.ProductPrice,
                    Quantity = qty, Category = p.Category?.CategoryName ?? "Khác" });
            else it.Quantity += qty;
        }
        public static void UpdateQuantity(int pid, int qty)
        {
            var it = GetCart().Items.FirstOrDefault(i => i.ProductID == pid);
            if (it != null) { it.Quantity = qty; if (it.Quantity <= 0) GetCart().Items.Remove(it); }
        }
        public static void RemoveItem(int pid) => GetCart().Items.RemoveAll(i => i.ProductID == pid);
        public static void Clear() => HttpContext.Current.Session[Key] = new Cart();
    }
}
