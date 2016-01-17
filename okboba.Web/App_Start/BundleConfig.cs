using AspNetBundling;
using System.Web;
using System.Web.Optimization;

namespace okboba
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/js/vendor").Include(
                "~/lib/jquery/jquery-{version}.js",
                "~/lib/jquery-ui/jquery-ui.js",
                "~/lib/jquery-validate/jquery.validate.js",
                "~/lib/jquery-validate/localization/messages*",
                //"~/lib/jquery-validate-unobtrusive/jquery.validate.unobtrusive.js",
                "~/lib/bootstrap/js/bootstrap.js",
                "~/lib/datepicker/js/bootstrap-datepicker.js",
                "~/lib/datepicker/locales/bootstrap-datepicker.zh-CN.min.js",
                "~/lib/filereader/filereader.js",
                "~/lib/jsviews/jsviews.js",
                "~/lib/masonry/masonry.pkgd.js",
                "~/lib/masonry/imagesloaded.pkgd.js",
                "~/lib/spinner/spin.js",
                "~/lib/spinner/jquery.spin.js",
                "~/lib/photoswipe/photoswipe.js",
                "~/lib/photoswipe/photoswipe-ui-default.js",
                //"~/lib/nicescroll/jquery.nicescroll.js",
                "~/lib/throttle/jquery.ba-throttle-debounce.js",
                "~/lib/signalr/jquery.signalR-2.2.0.js",
                //"~/lib/ladda/spin.min.js", //spinner already added above
                "~/lib/ladda-bootstrap/ladda.js",
                "~/lib/ladda-bootstrap/ladda.jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/okboba").Include(
                "~/Scripts/utilities.js",
                "~/Scripts/editinplace.js",
                "~/Scripts/locationpicker.js",
                "~/Scripts/matchscroller.js",
                "~/Scripts/photoupload.js",
                "~/Scripts/photomanager.js",
                "~/Scripts/photoswipe.js",
                "~/Scripts/questionvalidation.js",
                "~/Scripts/questionmanager.js",
                "~/Scripts/validation.js",
                "~/Scripts/chatslider.js",
                "~/Scripts/chatmanager.js",
                "~/Scripts/login.js",
                "~/Scripts/register.js",
                "~/Scripts/messaging.js",
                "~/Scripts/favorite.js"));

            bundles.Add(new StyleBundle("~/content/css").Include(
                "~/lib/bootstrap/css/bootstrap.css",
                "~/lib/bootstrap/css/bootstrap-theme.css",
                "~/lib/datepicker/css/bootstrap-datepicker3.css",
                "~/lib/jquery-ui/jquery-ui.css",
                "~/lib/photoswipe/photoswipe.css",
                "~/lib/photoswipe/default-skin/default-skin.css",
                "~/lib/ladda-bootstrap/ladda-themeless.css",
                "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/react").Include(
                "~/Scripts/react.js",
                "~/Scripts/react-dom.js"));
           
            bundles.Add(new StyleBundle("~/Content/admincss").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/themes/base/all.css",
                      "~/Content/admin.css"));
        }
    }
}
