using System.Linq;
using System.Web.Mvc;
using PagedList;
using TranHoangKhai_25DH190104_BE.Models;

namespace TranHoangKhai_25DH190104_BE.Areas.Admin.Controllers
{
    // Quan ly tai khoan Admin (CRUD + tim kiem + phan trang)
    public class AdminsController : AdminBaseController
    {
        public ActionResult Index(string keyword, int? page)
        {
            var q = db.Users.Where(u => u.UserRole == "A").AsQueryable();
            if (!string.IsNullOrEmpty(keyword)) q = q.Where(u => u.Username.Contains(keyword));
            ViewBag.keyword = keyword;
            return View(q.OrderBy(u => u.Username).ToPagedList(page ?? 1, 10));
        }
        public ActionResult Create() => View();
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(string Username, string Password)
        {
            if (!string.IsNullOrEmpty(Username) && Password?.Length >= 6)
            {
                if (db.Users.Any(u => u.Username == Username)) ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                else { db.Users.Add(new User { Username = Username, Password = Password, UserRole = "A" }); db.SaveChanges(); return RedirectToAction("Index"); }
            }
            else ModelState.AddModelError("", "Nhập tên đăng nhập và mật khẩu từ 6 ký tự.");
            return View();
        }
        public ActionResult Edit(string id) { var u = db.Users.Find(id); if (u == null) return HttpNotFound(); return View(u); }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(string Username, string Password)
        {
            var u = db.Users.Find(Username);
            if (u != null && !string.IsNullOrEmpty(Password)) { u.Password = Password; db.SaveChanges(); }
            return RedirectToAction("Index");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(string id) { var u = db.Users.Find(id); if (u != null && u.Username != "admin") { db.Users.Remove(u); db.SaveChanges(); } return RedirectToAction("Index"); }
    }
}
