using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace RD.SALV.APP.Pipeline
{
    public class RegisterApiRoute
    {
        public void Process(PipelineArgs args)
        {
            ///Register Route
            //RouteTable.Routes.MapHttpRoute("AzureLogViewer","api/AzureLogs/{controller}/{action}");

            var config = GlobalConfiguration.Configuration;
            config.Routes.MapHttpRoute("AzureLogViewer",
                                     "api/AzureLogs/{controller}/{action}");

        }
    }
}