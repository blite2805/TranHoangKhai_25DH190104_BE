using System.Web;
using System.Web.Optimization;

namespace _25DH190104_TranHoangKhai_BE
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            // Phần 3: bundle CSS dành cho Customer Site (2 file myStyle do agent Views tao + PagedList.css)
            bundles.Add(new StyleBundle("~/Content/customerCss").Include(
                      "~/Content/myStyle/CustomerHome.css",
                      "~/Content/myStyle/CustomerDetailProduct.css",
                      "~/Content/PagedList.css"));
        }
    }
}
