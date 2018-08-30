using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Idea.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new StyleBundle("~/Content/bundle")
                .Include("~/Content/bt/bootstrap.min.css", new CssRewriteUrlTransform()) // rewrite relative url to absolute url (show glyphicon)
                .Include(
                "~/Content/bt/dataTables.bootstrap.css",
                "~/Content/bt/datepicker.css",
              "~/Content/site.css"
              ));
            bundles.Add(new ScriptBundle("~/Scripts/bundle").Include(
                "~/Scripts/Include/modernizr-3.5.0.min.js", "~/Scripts/Include/plugins.js",
                "~/Scripts/Include/jquery-1.11.3.min.js", "~/Scripts/Include/bootstrap.min.js",
                "~/Scripts/Include/bootstrap-datepicker.js", "~/Scripts/Include/ie10-viewport-bug-workaround.js",
                "~/Scripts/Include/jquery.dataTables.min.js", "~/Scripts/Include/dataTables.bootstrap.js",
                "~/Scripts/Include/bootbox.min.js", "~/Scripts/Include/moment.min.js",
                "~/Scripts/Include/jquery.ui.widget.js", "~/Scripts/Include/jquery.iframe-transport.js",
                "~/Scripts/Include/jquery.fileupload.js"
                ));
            BundleTable.EnableOptimizations = true;
        }
    }
}