using System.ComponentModel.DataAnnotations;

namespace _25DH190104_TranHoangKhai_BE.Models
{
    // ViewModel cho trang đăng ký tài khoản khách hàng (Account/Register)
    // Áp dụng điều kiện validation: kiểm tra định dạng (Email/Regex), độ dài (StringLength), trùng Username (controller)
    public class RegisterVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tên đăng nhập chỉ gồm chữ và số")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có độ dài từ 6 đến 50 ký tự")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có độ dài từ 10 đến 15 ký tự")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số điện thoại chỉ gồm chữ số")]
        public string CustomerPhone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string CustomerEmail { get; set; }

        [StringLength(200)]
        public string CustomerAddress { get; set; }
    }
}
