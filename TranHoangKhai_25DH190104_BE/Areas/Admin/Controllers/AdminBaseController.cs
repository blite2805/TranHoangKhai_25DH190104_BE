using System.Linq;
using System.Web.Mvc;
using TranHoangKhai_25DH190104_BE.Models;

namespace TranHoangKhai_25DH190104_BE.Areas.Admin.Controllers
{
    // Kiem tra quyen Admin cho tat ca controller trong Area Admin
    public abstract class AdminBaseController : Controller
    {
        protected MyStoreEntities db = new MyStoreEntities();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Request.IsAuthenticated)
            { filterContext.Result = RedirectToAction("Login", "Account", new { area = "" }); return; }
            var u = db.Users.SingleOrDefault(x => x.Username == User.Identity.Name);
            if (u == null || u.UserRole.Trim() != "A")
            { filterContext.Result = RedirectToAction("Login", "Account", new { area = "" }); return; }
            base.OnActionExecuting(filterContext);
        }
        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
