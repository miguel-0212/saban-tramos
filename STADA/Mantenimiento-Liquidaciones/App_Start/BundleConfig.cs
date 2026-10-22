using System.Web;
using System.Web.Optimization;

namespace Mantenimiento_Liquidaciones
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/Jquery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/Jquery/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/Jquery/modernizr-*"));

            //Boostrap y css
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/Bootstrap/popper.min.js",
                        "~/Scripts/Bootstrap/bootstrap.min.js",
                        "~/Scripts/Bootstrap/respond.min.js",
                        "~/Scripts/*.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Bootstrap/bootstrap.min.css",
                      "~/Content/site.css",
                      "~/Style/style.min.css",
                      "~/Content/css/appCss/*.css",
                      "~/Content/css/*.css"));

            //OwnJs
            bundles.Add(new ScriptBundle("~/bundles/ownjs").Include(
                "~/Scripts/OwnJs/*.js",
                "~/Scripts/OwnJs/ScriptsApps/*.js",
                "~/Scripts/OwnJs/ScriptsApps/tablesKendo.js",
                "~/Scripts/Global.js"));

            bundles.Add(new StyleBundle("~/Content/Kendocss").Include(
                      "~/Content/kendo/variables.css",
                      "~/Content/kendo/stadaTheme.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/kendoUI").Include(
                        "~/Scripts/kendo/2021.1.224/angular.min.js",
                        "~/Scripts/kendo/2021.1.224/jszip.min.js",
                        "~/Scripts/kendo/2021.1.224/kendo.all.min.js",
                        "~/Scripts/kendo/2021.1.224/kendo.aspnetmvc.min.js",
                        "~/Scripts/kendo/2021.1.224/cultures/kendo.culture.es-ES.min.js",
                        "~/Scripts/kendo/2021.1.224/messages/kendo.messages.es-ES.min.js"
                        ));

            BundleTable.EnableOptimizations = false;

        }
    }
}
