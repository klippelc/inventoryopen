using System.Web.Optimization;

namespace Inventory.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //css
            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/bootstrap.css")
                    .Include("~/Content/PagedList.css", new CssRewriteUrlTransform())
                    .Include("~/Content/fontawesome-all.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/cssjqryUi").Include(
                     "~/Content/themes/base/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/css-bootstrap-select")
                   .Include("~/Content/bootstrap-select.min.css"));

            bundles.Add(new StyleBundle("~/Content/css-tagsinput")
                   .Include("~/Content/tagsinput.css"));

            //js
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                    "~/Scripts/umd/popper.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                   .Include("~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                      "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jsengine").Include(
                        "~/Scripts/apps/*.es5.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapselect")
                   .Include("~/Scripts/bootstrap-select.js"));

            bundles.Add(new ScriptBundle("~/bundles/tagsinput")
                   .Include("~/Scripts/tagsinput.js"));

        }
    }
}