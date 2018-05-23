using System.Web;
using System.Web.Optimization;

namespace WebNeYapsam
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/assets/plugins/bootstrap-social/bootstrap-social.css",
                      "~/assets/plugins/font-awesome/css/font-awesome.min.css",
                      "~/assets/plugins/simple-line-icons/simple-line-icons.min.css",
                      "~/assets/plugins/animate/animate.min.css",
                      "~/assets/plugins/bootstrap/css/bootstrap.min.css",
                      "~/assets/base/css/plugins.css",
                      "~/assets/base/css/components.css",
                      "~/assets/base/css/themes/default.css",
                      "~/assets/base/css/custom.css",
                      "~/vendor/sweetalert/lib/sweet-alert.css"
            ));

            bundles.Add(new ScriptBundle("~/Content/script").Include(
                "~/vendor/sweetalert/lib/sweet-alert.min.js",
                "~/assets/plugins/jquery.min.js",
                "~/assets/plugins/jquery-migrate.min.js",
                "~/assets/plugins/bootstrap/js/bootstrap.min.js",
                "~/assets/base/js/components.js",
                "~/assets/base/js/app.js"
            ));



            //......Panel Bundles

            //......Vendor Styles
            bundles.Add(new StyleBundle("~/Panel/vendor/css").Include(
                "~/vendor/fontawesome/css/font-awesome.css",
                "~/vendor/metisMenu/dist/metisMenu.css",
                "~/vendor/animate.css/animate.css",
                "~/vendor/bootstrap/dist/css/bootstrap.css",
                "~/vendor/sweetalert/lib/sweet-alert.css"
            ));
            //......Vendor Scripts
            bundles.Add(new ScriptBundle("~/Panel/vendor/script").Include(
                "~/vendor/sweetalert/lib/sweet-alert.min.js",
                "~/vendor/jquery/dist/jquery.min.js",
                "~/vendor/slimScroll/jquery.slimscroll.min.js",
                "~/vendor/jquery-ui/jquery-ui.min.js",
                "~/vendor/bootstrap/dist/js/bootstrap.min.js",
                "~/vendor/metisMenu/dist/metisMenu.min.js",
                "~/vendor/iCheck/icheck.min.js",
                "~/vendor/sparkline/index.js"

            ));

            //......App Styles
            bundles.Add(new StyleBundle("~/Panel/style").Include(
                "~/fonts/pe-icon-7-stroke/css/pe-icon-7-stroke.css",
                "~/fonts/pe-icon-7-stroke/css/helper.css",
                "~/Content/Panel/styles/style.css"
            ));

            //......App Scripts
            bundles.Add(new ScriptBundle("~/Panel/script").Include(
              "~/Scripts/Panel/homer.js"
            ));
        }
    }
}
