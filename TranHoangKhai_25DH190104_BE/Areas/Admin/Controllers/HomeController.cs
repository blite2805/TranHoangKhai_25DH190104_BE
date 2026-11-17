using System.Linq;
using System.Web.Mvc;

namespace TranHoangKhai_25DH190104_BE.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        // Trang chu Admin: thong ke (Google Chart)
        public ActionResult Index()
        {
            ViewBag.Customers = db.Customers.Count();
            ViewBag.Orders = db.Orders.Count();
            ViewBag.Revenue = db.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0;
            ViewBag.Products = db.Products.Count();
            // Doanh thu 7 ngay gan nhat (cho Google Chart)
            var rev = db.Orders.GroupBy(o => o.OrderDate)
                       .Select(g => new { d = g.Key, t = g.Sum(x => x.TotalAmount) })
                       .OrderByDescending(x => x.d).Take(7).OrderBy(x => x.d).ToList();
            ViewBag.RevLabels = string.Join(",", rev.Select(r => "'" + r.d.ToString("dd/MM") + "'"));
            ViewBag.RevValues = string.Join(",", rev.Select(r => (long)r.t));
            // Top 5 san pham ban chay
            ViewBag.Top = db.OrderDetails.GroupBy(o => o.ProductID)
                .Select(g => new { name = g.FirstOrDefault().Product.ProductName, qty = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.qty).Take(5).ToList();
            return View();
        }
    }
}
