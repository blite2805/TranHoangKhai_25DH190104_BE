using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TranHoangKhai_25DH190104_BE.Models;

namespace TranHoangKhai_25DH190104_BE.Areas.Admin.Controllers
{
    public class CategoriesController : AdminBaseController
    {
        public ActionResult Index() => View(db.Categories.ToList());
        public ActionResult Details(int? id) { if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest); var c = db.Categories.Find(id); if (c == null) return HttpNotFound(); return View(c); }
        public ActionResult Create() => View();
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName")] Category c) { if (ModelState.IsValid) { db.Categories.Add(c); db.SaveChanges(); return RedirectToAction("Index"); } return View(c); }
        public ActionResult Edit(int? id) { if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest); var c = db.Categories.Find(id); if (c == null) return HttpNotFound(); return View(c); }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName")] Category c) { if (ModelState.IsValid) { db.Entry(c).State = EntityState.Modified; db.SaveChanges(); return RedirectToAction("Index"); } return View(c); }
        public ActionResult Delete(int? id) { if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest); var c = db.Categories.Find(id); if (c == null) return HttpNotFound(); return View(c); }
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) { var c = db.Categories.Find(id); db.Categories.Remove(c); db.SaveChanges(); return RedirectToAction("Index"); }
    }
}
