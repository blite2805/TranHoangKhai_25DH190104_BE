using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using TranHoangKhai_25DH190104_BE.Models;

namespace TranHoangKhai_25DH190104_BE.Areas.Admin.Controllers
{
    public class CustomersController : AdminBaseController
    {
        // Xem, tim kiem, sap xep, phan trang tai khoan khach hang
        public ActionResult Index(string keyword, string sortOrder, int? page)
        {
            var q = db.Customers.Include(c => c.User).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                q = q.Where(c => c.CustomerName.Contains(keyword) || c.CustomerPhone.Contains(keyword) || c.Username.Contains(keyword));
            switch (sortOrder)
            {
                case "name_desc": q = q.OrderByDescending(c => c.CustomerName); break;
                case "spend": q = q.OrderByDescending(c => c.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0); break;
                default: q = q.OrderBy(c => c.CustomerName); break;
            }
            ViewBag.keyword = keyword; ViewBag.sortOrder = sortOrder;
            return View(q.ToPagedList(page ?? 1, 10));
        }

        // Chi tiet 1 khach hang: thong tin + danh sach don hang + chi tiet don hang
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var c = db.Customers.Include(x => x.Orders.Select(o => o.OrderDetails))
                                .SingleOrDefault(x => x.CustomerID == id);
            if (c == null) return HttpNotFound();
            return View(c);
        }
    }
}
