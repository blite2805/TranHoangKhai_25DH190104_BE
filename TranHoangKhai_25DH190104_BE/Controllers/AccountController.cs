using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using TranHoangKhai_25DH190104_BE.Models;
using TranHoangKhai_25DH190104_BE.Models.ViewModel;

namespace TranHoangKhai_25DH190104_BE.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM m)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Username == m.Username))
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                else
                {
                    db.Users.Add(new User { Username = m.Username, Password = m.Password, UserRole = "C" });
                    db.Customers.Add(new Customer {
                        CustomerName = m.CustomerName, CustomerPhone = m.CustomerPhone,
                        CustomerEmail = m.CustomerEmail, CustomerAddress = m.CustomerAddress, Username = m.Username });
                    db.SaveChanges();
                    FormsAuthentication.SetAuthCookie(m.Username, false);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(m);
        }

        public ActionResult Login(string returnUrl) { ViewBag.ReturnUrl = returnUrl; return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM m, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var u = db.Users.SingleOrDefault(x => x.Username == m.Username && x.Password == m.Password);
                if (u != null)
                {
                    FormsAuthentication.SetAuthCookie(u.Username, m.RememberMe);
                    if (u.UserRole.Trim() == "A") return RedirectToAction("Index", "Home", new { area = "Admin" });
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu.");
            }
            return View(m);
        }

        public ActionResult Logout() { FormsAuthentication.SignOut(); Session.Clear(); return RedirectToAction("Index", "Home"); }

        [Authorize]
        public ActionResult ProfileInfo()
        {
            var c = db.Customers.SingleOrDefault(x => x.Username == User.Identity.Name);
            return View(c);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Customer c)
        {
            var old = db.Customers.SingleOrDefault(x => x.Username == User.Identity.Name);
            if (old != null)
            {
                old.CustomerPhone = c.CustomerPhone; old.CustomerEmail = c.CustomerEmail; old.CustomerAddress = c.CustomerAddress;
                db.SaveChanges();
            }
            return RedirectToAction("ProfileInfo");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldPwd, string newPwd)
        {
            var u = db.Users.SingleOrDefault(x => x.Username == User.Identity.Name);
            if (u != null && u.Password == oldPwd && newPwd.Length >= 6) { u.Password = newPwd; db.SaveChanges(); }
            return RedirectToAction("ProfileInfo");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
