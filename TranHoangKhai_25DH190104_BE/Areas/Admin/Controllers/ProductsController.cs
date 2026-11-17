using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using TranHoangKhai_25DH190104_BE.Models;

namespace TranHoangKhai_25DH190104_BE.Areas.Admin.Controllers
{
    public class ProductsController : AdminBaseController
    {
        // CRUD + Tim kiem + Sap xep + Phan trang
        public ActionResult Index(string keyword, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var q = db.Products.Include(p => p.Category).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
                q = q.Where(p => p.ProductName.Contains(keyword) || p.ProductDecription.Contains(keyword) || p.Category.CategoryName.Contains(keyword));
            if (minPrice.HasValue) q = q.Where(p => p.ProductPrice >= minPrice);
            if (maxPrice.HasValue) q = q.Where(p => p.ProductPrice <= maxPrice);
            switch (sortOrder)
            {
                case "name_desc": q = q.OrderByDescending(p => p.ProductName); break;
                case "price_asc": q = q.OrderBy(p => p.ProductPrice); break;
                case "price_desc": q = q.OrderByDescending(p => p.ProductPrice); break;
                default: q = q.OrderBy(p => p.ProductName); break;
            }
            ViewBag.keyword = keyword; ViewBag.minPrice = minPrice; ViewBag.maxPrice = maxPrice; ViewBag.sortOrder = sortOrder;
            return View(q.ToPagedList(page ?? 1, 8));
        }

        public ActionResult Details(int? id) { if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest); var p = db.Products.Include(x => x.Category).SingleOrDefault(x => x.ProductID == id); if (p == null) return HttpNotFound(); return View(p); }
        public ActionResult Create() { ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName"); return View(); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage,UploadImg")] Product p)
        {
            if (ModelState.IsValid)
            {
                SaveImage(p);
                db.Products.Add(p); db.SaveChanges(); return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", p.CategoryID);
            return View(p);
        }

        public ActionResult Edit(int? id) { if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest); var p = db.Products.Find(id); if (p == null) return HttpNotFound(); ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", p.CategoryID); return View(p); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage,UploadImg")] Product p)
        {
            if (ModelState.IsValid)
            {
                SaveImage(p);
                db.Entry(p).State = EntityState.Modified; db.SaveChanges(); return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", p.CategoryID);
            return View(p);
        }

        public ActionResult Delete(int? id) { if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest); var p = db.Products.Include(x => x.Category).SingleOrDefault(x => x.ProductID == id); if (p == null) return HttpNotFound(); return View(p); }
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) { var p = db.Products.Find(id); db.Products.Remove(p); db.SaveChanges(); return RedirectToAction("Index"); }

        // Upload anh vao thu muc ~/Content/images
        private void SaveImage(Product p)
        {
            if (p.UploadImg != null && p.UploadImg.ContentLength > 0)
            {
                string filename = Path.GetFileName(p.UploadImg.FileName);
                string savePath = "~/Content/images/";
                p.ProductImage = Url.Content(savePath + filename);
                p.UploadImg.SaveAs(Path.Combine(Server.MapPath(savePath), filename));
            }
        }
    }
}
