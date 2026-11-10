using System.ComponentModel.DataAnnotations;

namespace _25DH190104_TranHoangKhai_BE.Models
{
    // ViewModel cho trang đăng nhập (Account/Login)
    public class LoginVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có độ dài từ 6 đến 50 ký tự")]
        public string Password { get; set; }
    }
}
