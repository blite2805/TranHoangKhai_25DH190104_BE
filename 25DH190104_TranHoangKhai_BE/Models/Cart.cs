using System.Collections.Generic;
using System.Linq;

namespace _25DH190104_TranHoangKhai_BE.Models
{
    // Lớp giỏ hàng: quản lý danh sách CartItem, nhóm theo CategoryName để hiển thị
    // AddItem cộng dồn Quantity nếu trùng ProductID (điều kiện validation "THÊM TRÙNG")
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);

        public int TotalQuantity => Items.Sum(i => i.Quantity);

        // Thêm sản phẩm vào giỏ: nếu trùng ProductID thì cộng dồn Quantity
        public void AddItem(CartItem item)
        {
            if (item == null) return;
            var existing = Items.FirstOrDefault(i => i.ProductID == item.ProductID);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.ProductID == productId);
        }

        public void UpdateQuantity(int productId, int qty)
        {
            var item = Items.FirstOrDefault(i => i.ProductID == productId);
            if (item == null) return;
            // quantity <= 0 -> xóa khỏi giỏ (điều kiện validation số lượng âm)
            if (qty <= 0)
            {
                RemoveItem(productId);
            }
            else
            {
                item.Quantity = qty;
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

        // Nhóm các mặt hàng theo CategoryName để view Cart/Index hiển thị theo danh mục
        public List<CartItemGroup> GroupedItems =>
            Items.GroupBy(i => i.CategoryName)
                 .Select(g => new CartItemGroup { CategoryName = g.Key, Items = g.ToList() })
                 .ToList();
    }

    // Nhóm mặt hàng theo danh mục (dùng cho view Cart/Index)
    public class CartItemGroup
    {
        public string CategoryName { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
