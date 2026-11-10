using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using _25DH190104_TranHoangKhai_BE.Models;

namespace _25DH190104_TranHoangKhai_BE.Controllers
{
    // CartController (viết lại Phần 3) — BỎ action Checkout / OrderSuccess (đã chuyển sang OrderController).
    // Giữ nguyên cơ chế UserCarts static (delegate qua CartService) để khôi phục giỏ khi login/logout.
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Giữ 2 method static để AccountController gọi (delegate sang CartService).
        public static void SyncUserCart(string username, List<CartItem> cart)
        {
            CartService.SyncUserCart(username, cart);
        }
        public static List<CartItem> GetUserCart(string username)
        {
            return CartService.GetUserCart(username);
        }

        // Helper: lấy giỏ từ Session qua CartService, đồng bộ vào UserCarts nếu đã login
        private List<CartItem> GetCart()
        {
            List<CartItem> cart = CartService.GetCart(Session);
            string username = Session["Username"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                CartService.SyncUserCart(username, cart);
            }
            Session["CartCount"] = cart.Sum(i => i.Quantity);
            return cart;
        }

        // Action đơn giản (giữ tên cũ / health-check)
        public ActionResult GetCartService()
        {
            return Content("ok");
        }

        // GET: Cart/Index — hiển thị giỏ hàng (view nhóm theo Category)
        public ActionResult Index()
        {
            List<CartItem> cart = GetCart();
            ViewBag.TotalAmount = cart.Sum(i => i.TotalPrice);
            // [1] CHUA CO DU LIEU: cart rỗng -> view hiển thị "Không có dữ liệu"
            return View(cart);
        }

        // POST: Cart/AddToCart — thêm sản phẩm vào giỏ (trùng ProductID thì cộng dồn Quantity)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            // [6] SO AM: quantity <= 0 -> từ chối, báo lỗi qua TempData
            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Số lượng phải lớn hơn 0.";
                return RedirectToAction("Index");
            }
            List<CartItem> cart = GetCart();
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
                    UnitPrice = Convert.ToDecimal(product.ProductPrice),
                    Quantity = quantity,
                    // Gán CategoryID/CategoryName để view nhóm giỏ theo danh mục
                    CategoryID = product.CategoryID,
                    CategoryName = product.Category != null ? product.Category.CategoryName : ""
                });
            }
            // Lưu Session + đồng bộ vào tài khoản nếu đã login
            CartService.SaveCart(Session, cart);
            string username = Session["Username"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                CartService.SyncUserCart(username, cart);
            }
            Session["CartCount"] = cart.Sum(i => i.Quantity);
            return RedirectToAction("Index");
        }

        // POST: Cart/RemoveFromCart — xóa một sản phẩm khỏi giỏ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromCart(int id)
        {
            List<CartItem> cart = GetCart();
            CartItem item = cart.FirstOrDefault(p => p.ProductID == id);
            if (item != null)
            {
                cart.Remove(item);
            }
            CartService.SaveCart(Session, cart);
            string username = Session["Username"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                CartService.SyncUserCart(username, cart);
            }
            Session["CartCount"] = cart.Sum(i => i.Quantity);
            return RedirectToAction("Index");
        }

        // GET: Cart/ClearCart — xóa toàn bộ giỏ hàng
        public ActionResult ClearCart()
        {
            List<CartItem> cart = GetCart();
            cart.Clear();
            CartService.SaveCart(Session, cart);
            string username = Session["Username"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                CartService.SyncUserCart(username, cart);
            }
            Session["CartCount"] = 0;
            return RedirectToAction("Index");
        }

        // POST: Cart/UpdateQuantity — cập nhật số lượng (quantity <= 0 -> xóa item)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            List<CartItem> cart = GetCart();
            CartItem item = cart.FirstOrDefault(p => p.ProductID == id);
            if (item != null)
            {
                // [6] SO AM: quantity <= 0 -> xóa item khỏi giỏ
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
            CartService.SaveCart(Session, cart);
            string username = Session["Username"] as string;
            if (!string.IsNullOrEmpty(username))
            {
                CartService.SyncUserCart(username, cart);
            }
            Session["CartCount"] = cart.Sum(i => i.Quantity);
            return RedirectToAction("Index");
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
