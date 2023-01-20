using System.Web.Optimization;

namespace OnlineRequestSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/jquery.validate.unobtrusive"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/idle.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/bootbox.js",
                        "~/Scripts/bootstrap-datepicker.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/ORTheme.css",
                        "~/Content/Site.css",
                        "~/Content/jquery-ui.css",
                        "~/Content/font-awesome.min.css",
                        "~/Content/bootstrap-datepicker.min.css",
                        "~/Content/SiteHelpers.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            // login layout
            bundles.Add(new ScriptBundle("~/bundles/bootstraplogin").Include(
                       "~/Scripts/bootstrap.js",
                       "~/Scripts/bootbox.js",
                       "~/Scripts/respond.js",
                       "~/Scripts/bootstrap-datepicker.min.js"));

            // login view
            bundles.Add(new ScriptBundle("~/bundles/loginview").Include(
                        "~/Scripts/Login/LocalStorageCleaner.js",
                        "~/Scripts/Login/loaderJS.js",
                        "~/Scripts/SourceNotAllowed.js"));

            // Open requests - multiple approving
            bundles.Add(new ScriptBundle("~/bundles/openrequests").Include(
                        "~/Scripts/OpenRequests/StateSaving.js",
                        "~/Scripts/MultiApproval/MultiApprove_AM.js",
                        "~/Scripts/MultiApproval/MultiApprove_RM.js",
                        "~/Scripts/MultiApproval/MultiApprove_DM.js",
                        "~/Scripts/MultiApproval/MultiApprove_Div.js",
                        "~/Scripts/MultiApproval/MultiApprove_VP.js",
                        "~/Scripts/MultiApproval/MultiApprove_President.js",
                        "~/Scripts/MultiApproval/MultipleDisapprove.js"));

            // Open requests - Third party libraries
            bundles.Add(new ScriptBundle("~/bundles/openrequestlibraries").Include(
                         "~/Scripts/notify.js",
                         "~/Scripts/table/jquery.dataTables.min.js",
                         "~/Scripts/table/dataTables.min.js",
                         "~/Scripts/bootbox.min.js"));

            // Request Details other third party libraries
            bundles.Add(new ScriptBundle("~/bundles/requestdetails").Include(
                        "~/Scripts/notify.js",
                        "~/Scripts/Request/RequestDetails.js",
                        "~/Scripts/bootbox.js",
                        "~/Scripts/Request/PrintFunctions.js"));

            // Request Details Ajax functions
            bundles.Add(new ScriptBundle("~/bundles/requestdetails_functions").Include(
                        "~/Scripts/Request/ItemOptions/MMDstatus.js",
                        "~/Scripts/Request/ItemOptions/DivStatus.js",
                        "~/Scripts/Request/ItemOptions/SDCStatus.js",
                        "~/Scripts/Request/ItemOptions/BranchStatus.js",
                        "~/Scripts/Request/CloseOrDisapprove.js",
                        "~/Scripts/Request/MMDButtons.js",
                        "~/Scripts/Request/RMButtons.js",
                        "~/Scripts/Request/RequestItems/BranchItems.js",
                        "~/Scripts/Request/RequestItems/DivItems.js",
                        "~/Scripts/Request/RequestItems/MMDItems.js",
                        "~/Scripts/Request/RequestItems/SDCItems.js",
                        "~/Scripts/Request/MoveToPending.js"));

            // create form view
            bundles.Add(new ScriptBundle("~/bundles/createformview_libraries").Include(
                        "~/Scripts/bootbox.js",
                        "~/Scripts/notify.js"));

            bundles.Add(new ScriptBundle("~/bundles/createformview_methods").Include(
                        "~/Scripts/Request/createformview.js",
                        "~/Scripts/Request/RequestFormView.js",
                        "~/Scripts/bootbox.js",
                        "~/Scripts/AlphaNumeric.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
}