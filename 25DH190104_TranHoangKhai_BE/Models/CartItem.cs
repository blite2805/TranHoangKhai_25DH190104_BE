using System;

namespace _25DH190104_TranHoangKhai_BE.Models
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        // Thêm cho Phần 3: dùng để nhóm giỏ theo danh mục (Cart.GroupedItems)
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        // Tính thành tiền cho từng sản phẩm (get-only)
        public decimal TotalPrice
        {
            get { return Quantity * UnitPrice; }
        }
    }
}
