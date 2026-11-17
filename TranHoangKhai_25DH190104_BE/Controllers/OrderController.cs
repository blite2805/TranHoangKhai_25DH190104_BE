using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TranHoangKhai_25DH190104_BE.Models;
using TranHoangKhai_25DH190104_BE.Models.ViewModel;

namespace TranHoangKhai_25DH190104_BE.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Thanh toan: hien form (GET)
        public ActionResult Checkout()
        {
            var cart = CartService.GetCart();
            if (cart.Items.Count == 0) return RedirectToAction("Index", "Home");
            var c = db.Customers.SingleOrDefault(x => x.Username == User.Identity.Name);
            var vm = new CheckoutVM
            {
                ShippingAddress = c?.CustomerAddress ?? "",
                DeliveryMethod = "Giao hàng tận nơi",
                PaymentMethod = "Thanh toán khi nhận hàng (COD)",
                Total = cart.Total
            };
            return View(vm);
        }

        // Dat hang: luu Order + OrderDetail vao DB (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM m)
        {
            var cart = CartService.GetCart();
            if (cart.Items.Count == 0) return RedirectToAction("Index", "Home");
            var customer = db.Customers.SingleOrDefault(x => x.Username == User.Identity.Name);
            if (customer == null) return RedirectToAction("Login", "Account");

            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Total,
                PaymentStatus = "Pending",
                ShippingAddress = string.IsNullOrEmpty(m.ShippingAddress) ? (customer.CustomerAddress ?? "") : m.ShippingAddress,
                DeliveryMethod = m.DeliveryMethod,
                PaymentMethod = m.PaymentMethod,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var it in cart.Items)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductID = it.ProductID,
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice
                });
            }

            db.Orders.Add(order);
            db.SaveChanges();
            CartService.Clear();
            return RedirectToAction("OrderSuccess", new { id = order.OrderID });
        }

        // Xac nhan don hang
        public ActionResult OrderSuccess(int? id)
        {
            if (id == null) return RedirectToAction("MyOrder");
            var order = db.Orders.Include("OrderDetails.Product").Include("Customer").SingleOrDefault(o => o.OrderID == id);
            return View(order);
        }

        // Lich su mua hang cua khach dang nhap
        public ActionResult MyOrder(string search)
        {
            var customer = db.Customers.SingleOrDefault(x => x.Username == User.Identity.Name);
            if (customer == null) return RedirectToAction("Login", "Account");
            var orders = db.Orders.Include("OrderDetails.Product").Where(o => o.CustomerID == customer.CustomerID).OrderByDescending(o => o.OrderDate).AsQueryable();
            if (!string.IsNullOrEmpty(search))
                orders = orders.Where(o => o.OrderID.ToString() == search || o.OrderDetails.Any(d => d.Product.ProductName.Contains(search)));
            return View(orders.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}