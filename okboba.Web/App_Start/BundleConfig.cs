using System.Web;
using System.Web.Optimization;

namespace okboba
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js/vendor").Include(
                "~/lib/jquery/jquery-{version}.js",
                "~/lib/jquery-ui/jquery-ui.js",
                "~/lib/jquery-validate/jquery.validate.js",
                "~/lib/jquery-validate/localization/messages*",
                "~/lib/jquery-validate-unobtrusive/jquery.validate.unobtrusive.js",
                "~/lib/bootstrap/js/bootstrap.js",
                "~/lib/datepicker/js/bootstrap-datepicker.js",
                "~/lib/datepicker/locales/bootstrap-datepicker.*",
                "~/lib/filereader/filereader.js",
                "~/lib/jsviews/jsviews.js",
                "~/lib/masonry/masonry.pkgd.js",
                "~/lib/masonry/imagesloaded.pkgd.js",
                "~/lib/spinner/spin.js",
                "~/lib/spinner/jquery.spin.js",
                "~/lib/photoswipe/photoswipe.js",
                "~/lib/photoswipe/photoswipe-ui-default.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/okboba").Include(
                "~/Scripts/utilities.js",
                "~/Scripts/editinplace.js",
                "~/Scripts/locationpicker.js",
                "~/Scripts/matchscroller.js",
                "~/Scripts/photoupload2.js",
                "~/Scripts/editthumbnail.js"));

            bundles.Add(new StyleBundle("~/bundles/css/all").Include(
                "~/lib/bootstrap/css/bootstrap.css",
                "~/lib/bootstrap/css/bootstrap-theme.css",
                "~/lib/datepicker/css/bootstrap-datepicker3.css",
                "~/lib/jquery-ui/jquery-ui.css",
                "~/lib/photoswipe/photoswipe.css",
                "~/lib/photoswipe/default-skin/default-skin.css",
                "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/react").Include(
                "~/Scripts/react.js",
                "~/Scripts/react-dom.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js",
            //            "~/Scripts/jquery-ui-{version}.js",
            //            "~/Scripts/spin.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //bundles.Add(new ScriptBundle("~/bundles/filereader").Include(
            //            "~/Scripts/filereader.js"));

            //bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
            //    "~/Scripts/bootstrap-datepicker.js",
            //    "~/Scripts/locales/bootstrap-datepicker.zh-CN.min.js"));            

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/okboba").Include(
            //    "~/Scripts/okboba.js"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/themes/base/all.css",
            //          "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/admincss").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/themes/base/all.css",
                      "~/Content/admin.css"));
        }
    }
}
