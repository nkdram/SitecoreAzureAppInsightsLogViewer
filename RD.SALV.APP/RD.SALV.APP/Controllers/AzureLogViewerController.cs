using RD.SALV.CORE.Models;
using RD.SALV.CORE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.Script.Serialization;

namespace RD.SALV.APP.Controllers
{
    [System.Web.Mvc.ValidateInput(false)]
    public class AzureLogViewerController : ApiController
    {
        private ILogViewerService _logViewerService { get; set; }

        public AzureLogViewerController()
        {
            var appId = Sitecore.Configuration.Settings.GetSetting("Azure.Appinsights.AppId");
            var apiKey = Sitecore.Configuration.Settings.GetSetting("Azure.Appinsights.AppKey");
            _logViewerService = new AzureLogViewerService(appId, apiKey);
        }

        
        //// GET: AzureLogViewer
        [System.Web.Http.HttpPost]
        [ResponseType(typeof(LogResponseData))]
        public IHttpActionResult GetLogData(LogRequestData logRequestData)
        {           
            //Get logs based on requested data
            return Ok(_logViewerService.GetLogs(logRequestData));
        }

        [System.Web.Http.HttpGet,HttpPost]
        public IHttpActionResult GetCloudInstances()
        {
            //Get logs based on requested data
            return Ok(_logViewerService.GetCloudInstances());
        }
    }
}