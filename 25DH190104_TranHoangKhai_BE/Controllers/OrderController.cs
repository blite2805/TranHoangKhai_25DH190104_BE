using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using _25DH190104_TranHoangKhai_BE.Models;

namespace _25DH190104_TranHoangKhai_BE.Controllers
{
    // OrderController (mới, Phần 3) — tách Checkout/OrderSuccess khỏi Cart, thêm MyOrder/OrderDetail.
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Helper: lấy Customer của phiên đăng nhập hiện tại (từ Session hoặc truy vấn DB)
        private Customer GetCurrentCustomer()
        {
            string username = Session["Username"] as string;
            if (string.IsNullOrEmpty(username)) return null;
            var customer = Session["Customer"] as Customer;
            if (customer != null) return customer;
            return db.Customers.FirstOrDefault(c => c.Username == username);
        }

        // GET: Order/Checkout — chưa login -> Login; giỏ rỗng -> Home Index; tạo CheckoutVM mặc định
        public ActionResult Checkout()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<CartItem> cart = CartService.GetCart(Session);
            // [1] CHUA CO DU LIEU: giỏ rỗng -> chuyển về trang chủ
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            Customer customer = GetCurrentCustomer();
            CheckoutVM model = new CheckoutVM
            {
                CustomerID = customer != null ? customer.CustomerID : 0,
                CustomerName = customer != null ? customer.CustomerName : "",
                CustomerPhone = customer != null ? customer.CustomerPhone : "",
                CustomerEmail = customer != null ? customer.CustomerEmail : "",
                // Mặc định địa chỉ giao hàng = địa chỉ khách hàng (CustomerAddress)
                ShippingAddress = customer != null ? customer.CustomerAddress : "",
                TotalAmount = cart.Sum(i => i.TotalPrice),
                Items = cart
            };
            return View(model);
        }

        // POST: Order/Checkout — tạo Order + OrderDetail, xóa giỏ, redirect OrderSuccess
        // [4] DINH DANG / [5] DO DAI: ShippingAddress [Required][StringLength(200)] trong CheckoutVM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            List<CartItem> cart = CartService.GetCart(Session);
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                // Khôi phục lại Items + tổng tiền để hiển thị lại form
                model.Items = cart;
                model.TotalAmount = cart.Sum(i => i.TotalPrice);
                return View(model);
            }
            Customer customer = GetCurrentCustomer();
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Tạo Order mới (PaymentStatus mặc định "Chưa thanh toán")
            Order order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(i => i.TotalPrice),
                ShippingAddress = model.ShippingAddress,
                DeliveryMethod = model.DeliveryMethod,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = "Chưa thanh toán"
            };
            db.Orders.Add(order);
            db.SaveChanges();
            // Tạo OrderDetail cho từng mặt hàng (TotalPrice là computed column PERSISTED — KHÔNG gán giá trị)
            foreach (var item in cart)
            {
                OrderDetail detail = new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
                db.OrderDetails.Add(detail);
            }
            db.SaveChanges();
            // Xóa giỏ: Session Cart + CartCount + UserCarts (static dictionary)
            string username = Session["Username"].ToString();
            CartService.ClearCart(Session);
            Session["CartCount"] = 0;
            if (CartService.UserCarts.ContainsKey(username))
            {
                CartService.UserCarts.Remove(username);
            }
            return RedirectToAction("OrderSuccess", new { id = order.OrderID });
        }

        // GET: Order/OrderSuccess — hiển thị đơn hàng vừa đặt
        public ActionResult OrderSuccess(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Order order = db.Orders.Find(id.Value);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Order/MyOrder — lịch sử đơn hàng của customer đang login, lọc theo status + searchTerm
        public ActionResult MyOrder(string status, string searchTerm)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Customer customer = GetCurrentCustomer();
            // [1] CHUA CO DU LIEU: không tìm thấy customer -> trả list rỗng (view hiển thị "Không có dữ liệu")
            if (customer == null)
            {
                ViewBag.CurrentStatus = status;
                ViewBag.SearchTerm = searchTerm;
                return View(new List<Order>());
            }
            var orders = db.Orders.Where(o => o.CustomerID == customer.CustomerID).AsQueryable();
            // Lọc theo tab trạng thái (PaymentStatus): "Tất cả" = không lọc
            if (!string.IsNullOrEmpty(status) && !string.Equals(status, "Tất cả", StringComparison.OrdinalIgnoreCase))
            {
                orders = orders.Where(o => o.PaymentStatus == status);
            }
            // Tìm kiếm theo OrderID hoặc ShippingAddress
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string term = searchTerm.Trim();
                orders = orders.Where(o => o.ShippingAddress.Contains(term) || o.OrderID.ToString().Contains(term));
            }
            var result = orders.OrderByDescending(o => o.OrderDate).ToList();
            // [1] CHUA CO DU LIEU: nếu rỗng, view hiển thị "Không có dữ liệu"
            ViewBag.CurrentStatus = status;
            ViewBag.SearchTerm = searchTerm;
            return View(result);
        }

        // GET: Order/OrderDetail — chi tiết đơn hàng, kiểm tra đơn thuộc về customer đang login
        public ActionResult OrderDetail(int id)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Customer customer = GetCurrentCustomer();
            if (customer == null)
            {
                return RedirectToAction("MyOrder");
            }
            // Kiểm tra đơn hàng thuộc về customer đang login (tránh xem đơn của người khác)
            Order order = db.Orders.FirstOrDefault(o => o.OrderID == id && o.CustomerID == customer.CustomerID);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderDetails = db.OrderDetails.Where(d => d.OrderID == id).ToList();
            return View(order);
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
