using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _25DH190104_TranHoangKhai_BE.Models
{
    // ViewModel cho trang thanh toán (Order/Checkout)
    // Chỉ ShippingAddress bắt buộc theo spec; Phone/Email kiểm tra định dạng nếu có nhập
    public class CheckoutVM
    {
        public int CustomerID { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có độ dài từ 10 đến 15 ký tự")]
        public string CustomerPhone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string CustomerEmail { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [StringLength(200)]
        public string ShippingAddress { get; set; }

        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }

        public decimal TotalAmount { get; set; }

        public List<CartItem> Items { get; set; }
    }
}
