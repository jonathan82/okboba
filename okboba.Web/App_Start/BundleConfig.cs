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
                "~/lib/datepicker/locales/bootstrap-datepicker.zh-CN.min.js",
                "~/lib/filereader/filereader.js",
                "~/lib/jsviews/jsviews.js",
                "~/lib/masonry/masonry.pkgd.js",
                "~/lib/masonry/imagesloaded.pkgd.js",
                "~/lib/spinner/spin.js",
                "~/lib/spinner/jquery.spin.js",
                "~/lib/photoswipe/photoswipe.js",
                "~/lib/photoswipe/photoswipe-ui-default.js",
                "~/lib/nicescroll/jquery.nicescroll.js",
                "~/lib/throttle/jquery.ba-throttle-debounce.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/okboba").Include(
                "~/Scripts/utilities.js",
                "~/Scripts/editinplace.js",
                "~/Scripts/locationpicker.js",
                "~/Scripts/matchscroller.js",
                "~/Scripts/photoupload.js",
                "~/Scripts/editthumbnail.js",
                "~/Scripts/photoswipe.js",
                "~/Scripts/question.js",
                "~/Scripts/validation.js",
                "~/Scripts/chat.js"));

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
           
            bundles.Add(new StyleBundle("~/Content/admincss").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/themes/base/all.css",
                      "~/Content/admin.css"));
        }
    }
}
