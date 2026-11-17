using System.Linq;
using System.Web.Mvc;
using PagedList;
using TranHoangKhai_25DH190104_BE.Models;
using TranHoangKhai_25DH190104_BE.Models.ViewModel;

namespace TranHoangKhai_25DH190104_BE.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Gio hang nang cao: nhom theo danh muc + san pham tuong tu
        public ActionResult Index(int? page)
        {
            var cart = CartService.GetCart();
            var catIds = cart.Items.Select(i => i.ProductID)
                .Select(pid => db.Products.Find(pid)?.CategoryID).Distinct().ToList();
            cart.SimilarProducts = db.Products.Where(p => catIds.Contains(p.CategoryID))
                .OrderByDescending(p => p.ProductID).ToPagedList(page ?? 1, 4);
            return View(cart);
        }

        [HttpPost]
        public ActionResult AddItem(int id, int quantity = 1)
        {
            var p = db.Products.Find(id);
            if (p != null) CartService.AddItem(p, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity) { CartService.UpdateQuantity(id, quantity); return RedirectToAction("Index"); }

        [HttpPost]
        public ActionResult RemoveItem(int id) { CartService.RemoveItem(id); return RedirectToAction("Index"); }

        [HttpPost]
        public ActionResult ClearCart() { CartService.Clear(); return RedirectToAction("Index"); }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
