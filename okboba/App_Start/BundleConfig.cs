using System.Web;
using System.Web.Optimization;

namespace okboba
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/filereader").Include(
                        "~/Scripts/filereader.js"));

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/locales/bootstrap-datepicker.zh-CN.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/okboba").Include(
                "~/Scripts/okboba.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/themes/base/all.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/datepicker").Include(
                "~/Content/datepicker/bootstrap-datepicker.css",
                "~/Content/datepicker/bootstrap-datepicker3.css"));
        }
    }
}
