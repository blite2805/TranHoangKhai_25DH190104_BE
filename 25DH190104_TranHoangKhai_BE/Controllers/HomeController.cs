using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using _25DH190104_TranHoangKhai_BE.Models;

namespace _25DH190104_TranHoangKhai_BE.Controllers
{
    // HomeController (viết lại Phần 3) — Index (HomeProductVM), ProductList, ProductDetail (GET+POST), About, Contact.
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Home/Index — trang chủ: FeaturedProducts top 10 bán chạy + NewProducts 20 sp ít người mua (phân trang 6)
        public ActionResult Index(string searchTerm, int? page)
        {
            var featuredQuery = db.Products.AsQueryable();
            var newQuery = db.Products.AsQueryable();
            // Lọc searchTerm cho cả hai danh sách theo ProductName
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string term = searchTerm.Trim();
                featuredQuery = featuredQuery.Where(p => p.ProductName.Contains(term));
                newQuery = newQuery.Where(p => p.ProductName.Contains(term));
                ViewBag.SearchTerm = searchTerm;
            }
            // FeaturedProducts: top 10 bán chạy theo tổng Quantity trong OrderDetails
            List<Product> featured = featuredQuery
                .OrderByDescending(p => p.OrderDetails.Sum(d => (int?)d.Quantity) ?? 0)
                .Take(10)
                .ToList();
            // NewProducts: 20 sp ít người mua, phân trang 6/trang
            var newProducts = newQuery
                .OrderBy(p => p.OrderDetails.Sum(d => (int?)d.Quantity) ?? 0)
                .Take(20)
                .ToPagedList(page ?? 1, 6);
            ViewBag.Categories = db.Categories.ToList();
            HomeProductVM vm = new HomeProductVM
            {
                FeaturedProducts = featured,
                NewProducts = newProducts
            };
            return View(vm);
        }

        // GET: Home/ProductList — danh sách sản phẩm: lọc theo category/search, sắp xếp, phân trang 6/trang
        public ActionResult ProductList(int? categoryId, string searchTerm, string sortOrder, int? page)
        {
            IQueryable<Product> products = db.Products.Include(p => p.Category);
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryID == categoryId.Value);
                ViewBag.SelectedCategory = categoryId.Value;
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string term = searchTerm.Trim();
                products = products.Where(p => p.ProductName.Contains(term));
                ViewBag.SearchTerm = searchTerm;
            }
            // Sắp xếp: price_asc / price_desc / newest (mặc định theo tên)
            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                case "newest":
                    products = products.OrderByDescending(p => p.ProductID);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.Categories = db.Categories.ToList();
            // [1] CHUA CO DU LIEU: nếu rỗng, view hiển thị "Không có dữ liệu"
            return View(products.ToPagedList(page ?? 1, 6));
        }

        // GET: Home/ProductDetail — chi tiết sản phẩm + RelatedProducts 8 sp cùng danh mục + TopProducts 8 sp bán chạy cùng danh mục
        [HttpGet]
        public ActionResult ProductDetail(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            // RelatedProducts: 8 sản phẩm cùng danh mục (khác id hiện tại)
            var related = db.Products
                .Where(p => p.CategoryID == product.CategoryID && p.ProductID != id)
                .Take(8)
                .ToList();
            // TopProducts: 8 sản phẩm bán chạy nhất cùng danh mục
            var top = db.Products
                .Where(p => p.CategoryID == product.CategoryID && p.ProductID != id)
                .OrderByDescending(p => p.OrderDetails.Sum(d => (int?)d.Quantity) ?? 0)
                .Take(8)
                .ToList();
            ProductDetailVM vm = new ProductDetailVM
            {
                Product = product,
                RelatedProducts = related,
                TopProducts = top
            };
            return View(vm);
        }

        // POST: Home/ProductDetail — thêm vào giỏ từ trang chi tiết, rồi chuyển sang Cart/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductDetail(int id, int quantity = 1, int? page = null)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            // [6] SO AM: quantity <= 0 -> báo lỗi, quay lại trang chi tiết
            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Số lượng phải lớn hơn 0.";
                return RedirectToAction("ProductDetail", new { id = id });
            }
            List<CartItem> cart = CartService.GetCart(Session);
            // [3] THEM TRUNG: trùng ProductID -> cộng dồn Quantity
            CartItem existing = cart.FirstOrDefault(p => p.ProductID == id);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    ProductImage = product.ProductImage,
                    UnitPrice = System.Convert.ToDecimal(product.ProductPrice),
                    Quantity = quantity,
                    CategoryID = product.CategoryID,
                    CategoryName = product.Category != null ? product.Category.CategoryName : ""
                });
            }
            CartService.SaveCart(Session, cart);
            string username = Session["Username"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                CartService.SyncUserCart(username, cart);
            }
            Session["CartCount"] = cart.Sum(i => i.Quantity);
            return RedirectToAction("Index", "Cart");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
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
