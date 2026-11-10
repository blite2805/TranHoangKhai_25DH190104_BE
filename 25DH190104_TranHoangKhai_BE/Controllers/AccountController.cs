using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using _25DH190104_TranHoangKhai_BE.Models;

namespace _25DH190104_TranHoangKhai_BE.Controllers
{
    // AccountController (Phần 3) — thay thế UserController.
    // Chịu trách nhiệm: Register, Login, Logout, ProfileInfo, UpdateProfile, ChangePassword.
    // KHÔNG có OrderHistory / OrderDetail (đã chuyển sang OrderController).
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Account/Register — hiện trang đăng ký (RegisterVM)
        [HttpGet]
        public ActionResult Register()
        {
            return View(new RegisterVM());
        }

        // POST: Account/Register
        // [4] DINH DANG / [5] DO DAI: kiểm tra bởi DataAnnotation trong RegisterVM
        //      (Regex Username chỉ chữ+số, Regex SĐT chỉ chữ số, [EmailAddress], [StringLength])
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // [3] THEM TRUNG: kiểm tra trùng Username trước khi tạo tài khoản
            if (db.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                return View(model);
            }
            // Tạo User với vai trò khách hàng (UserRole = "C")
            User usr = new User
            {
                Username = model.Username.Trim(),
                Password = model.Password,
                UserRole = "C"
            };
            // Tắt validate khi lưu User (tránh lỗi metadata EF với bảng User)
            db.Configuration.ValidateOnSaveEnabled = false;
            db.Users.Add(usr);
            db.SaveChanges();
            // Tạo Customer liên kết với User vừa tạo
            Customer cus = new Customer
            {
                CustomerName = model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                CustomerEmail = model.CustomerEmail,
                CustomerAddress = model.CustomerAddress,
                Username = usr.Username
            };
            db.Customers.Add(cus);
            db.SaveChanges();
            db.Configuration.ValidateOnSaveEnabled = true;
            return RedirectToAction("Login");
        }

        // GET: Account/Login — hiện trang đăng nhập (LoginVM)
        [HttpGet]
        public ActionResult Login()
        {
            return View(new LoginVM());
        }

        // POST: Account/Login — kiểm tra user/pass, SetAuthCookie + Session, khôi phục giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userCheck = db.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            if (userCheck == null)
            {
                // Sai tên đăng nhập hoặc mật khẩu
                ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu.");
                return View(model);
            }
            // Đăng nhập thành công: FormsAuthentication cookie + các Session cần thiết
            FormsAuthentication.SetAuthCookie(model.Username, false);
            Session["Username"] = userCheck.Username;
            Session["UserRole"] = userCheck.UserRole;
            var customerInfo = db.Customers.FirstOrDefault(c => c.Username == userCheck.Username);
            if (customerInfo != null)
            {
                Session["CustomerName"] = customerInfo.CustomerName;
                Session["Customer"] = customerInfo;
            }
            // KHÔI PHỤC giỏ hàng của tài khoản từ CartService.UserCarts (static)
            List<CartItem> savedCart = CartService.GetUserCart(userCheck.Username);
            if (savedCart != null && savedCart.Count > 0)
            {
                Session["Cart"] = savedCart;
                Session["CartCount"] = savedCart.Sum(i => i.Quantity);
            }
            else
            {
                // Nếu trước khi login đã có giỏ tạm trong Session, lưu luôn vào tài khoản
                List<CartItem> currentCart = Session["Cart"] as List<CartItem>;
                if (currentCart != null && currentCart.Count > 0)
                {
                    CartService.SyncUserCart(userCheck.Username, currentCart);
                    Session["CartCount"] = currentCart.Sum(i => i.Quantity);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Logout — SyncUserCart, xóa Session, SignOut
        public ActionResult Logout()
        {
            // Trước khi xóa Session, lưu giỏ hàng vào tài khoản (khôi phục được lần sau)
            string username = Session["Username"] as string;
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (!string.IsNullOrEmpty(username))
            {
                CartService.SyncUserCart(username, cart);
            }
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/ProfileInfo — hiển thị Customer để sửa SĐT/Email/Địa chỉ
        [HttpGet]
        public ActionResult ProfileInfo()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login");
            }
            string username = Session["Username"].ToString();
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View("ProfileInfo", customer);
        }

        // POST: Account/UpdateProfile — chỉ cập nhật CustomerName/Phone/Email/Address
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Customer model)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login");
            }
            string username = Session["Username"].ToString();
            // [6] NULL: lấy Customer theo username phiên login (không tin CustomerID post từ client)
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null)
            {
                return HttpNotFound();
            }
            if (!ModelState.IsValid)
            {
                return View("ProfileInfo", model);
            }
            // Chỉ cập nhật 4 trường cho phép: CustomerName, Phone, Email, Address
            customer.CustomerName = model.CustomerName;
            customer.CustomerPhone = model.CustomerPhone;
            customer.CustomerEmail = model.CustomerEmail;
            customer.CustomerAddress = model.CustomerAddress;
            db.SaveChanges();
            // Cập nhật lại Session sau khi lưu DB
            Session["CustomerName"] = customer.CustomerName;
            Session["Customer"] = customer;
            // [6] TempData: thông báo thành công
            TempData["SuccessMessage"] = "Cập nhật thông tin tài khoản thành công!";
            return RedirectToAction("ProfileInfo");
        }

        // POST: Account/ChangePassword — kiểm tra mật khẩu cũ rồi cập nhật mật khẩu mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login");
            }
            string username = Session["Username"].ToString();
            var user = db.Users.Find(username);
            if (user == null)
            {
                return HttpNotFound();
            }
            // [6] NULL/so sánh: mật khẩu cũ không khớp
            if (user.Password != oldPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu cũ không đúng.";
                return RedirectToAction("ProfileInfo");
            }
            // [5] DO DAI: mật khẩu mới tối thiểu 6 ký tự
            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return RedirectToAction("ProfileInfo");
            }
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction("ProfileInfo");
            }
            user.Password = newPassword;
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("ProfileInfo");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
