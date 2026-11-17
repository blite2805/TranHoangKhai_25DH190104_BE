using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using TranHoangKhai_25DH190104_BE.Models;
using TranHoangKhai_25DH190104_BE.Models.ViewModel;

namespace TranHoangKhai_25DH190104_BE.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Trang chu: banner + tim kiem + san pham noi bat (top 10) + san pham moi (phan trang 6/trang)
        public ActionResult Index(string searchTerm, int? page)
        {
            var q = db.Products.Include(p => p.Category).AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
                q = q.Where(p => p.ProductName.Contains(searchTerm) || p.ProductDecription.Contains(searchTerm) || p.Category.CategoryName.Contains(searchTerm));

            // Top 10 ban chay nhat (theo so luong trong OrderDetail)
            var sold = db.OrderDetails.GroupBy(o => o.ProductID)
                        .Select(g => new { pid = g.Key, total = g.Sum(x => x.Quantity) })
                        .OrderByDescending(x => x.total).Take(10).ToList();
            var featured = sold.Select(s => db.Products.Find(s.pid)).Where(p => p != null).ToList();
            if (featured.Count < 4) featured = q.OrderByDescending(p => p.ProductID).Take(10).ToList();

            var vm = new HomeProductVM {
                SearchTerm = searchTerm,
                FeaturedProducts = featured,
                NewProducts = q.OrderByDescending(p => p.ProductID).ToPagedList(page ?? 1, 6)
            };
            ViewBag.Categories = db.Categories.ToList();
            return View(vm);
        }

        // Danh sach san pham theo danh muc
        public ActionResult ProductList(int? id, string searchTerm, int? page)
        {
            var q = db.Products.Include(p => p.Category).AsQueryable();
            if (id.HasValue) q = q.Where(p => p.CategoryID == id);
            if (!string.IsNullOrEmpty(searchTerm))
                q = q.Where(p => p.ProductName.Contains(searchTerm) || p.ProductDecription.Contains(searchTerm));
            ViewBag.CategoryID = id; ViewBag.Categories = db.Categories.ToList(); ViewBag.SearchTerm = searchTerm;
            return View(q.OrderByDescending(p => p.ProductID).ToPagedList(page ?? 1, 8));
        }

        // Chi tiet san pham (GET)
        public ActionResult ProductDetail(int? id, int? page)
        {
            if (id == null) return RedirectToAction("Index");
            var p = db.Products.Include(x => x.Category).SingleOrDefault(x => x.ProductID == id);
            if (p == null) return HttpNotFound();
            var vm = new ProductDetailVM {
                Product = p,
                RelatedProducts = db.Products.Where(x => x.CategoryID == p.CategoryID && x.ProductID != p.ProductID).Take(8).ToList(),
                TopProducts = db.Products.Where(x => x.CategoryID == p.CategoryID && x.ProductID != p.ProductID)
                                .OrderByDescending(x => x.ProductID).ToPagedList(page ?? 1, 3)
            };
            ViewBag.Categories = db.Categories.ToList();
            return View(vm);
        }

        // Them vao gio (POST tu trang chi tiet)
        [HttpPost]
        public ActionResult ProductDetail(int id, int quantity)
        {
            var p = db.Products.Find(id);
            if (p != null) CartService.AddItem(p, quantity > 0 ? quantity : 1);
            return RedirectToAction("Index", "Cart");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
