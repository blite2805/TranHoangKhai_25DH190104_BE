using System.ComponentModel.DataAnnotations;

namespace TranHoangKhai_25DH190104_BE.Models.ViewModel
{
    // Dang ky: ket hop User + Customer
    public class RegisterVM
    {
        [Required(ErrorMessage = "Nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Nhập số điện thoại")]
        [RegularExpression(@"^[0-9+\s]{9,15}$", ErrorMessage = "SĐT 9-15 chữ số")]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Địa chỉ")]
        public string CustomerAddress { get; set; }

        [Required(ErrorMessage = "Nhập tên đăng nhập")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Tên đăng nhập 3-20 ký tự")]
        [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "Chỉ gồm chữ, số, dấu _")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        [Display(Name = "Nhập lại mật khẩu")]
        public string ConfirmPassword { get; set; }
    }

    // Dang nhap
    public class LoginVM
    {
        [Required(ErrorMessage = "Nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }
}
