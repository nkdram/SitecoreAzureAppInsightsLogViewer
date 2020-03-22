using RD.SALV.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.SALV.CORE.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILogViewerService
    {
        /// <summary>
        /// Get Logs using Log ReqData
        /// </summary>
        /// <param name="LogreqData"></param>
        /// <returns></returns>
        LogResponseData GetLogs(LogRequestData LogreqData);

        /// <summary>
        /// Returns cloud instances
        /// </summary>
        /// <returns></returns>
        CloudInstanceResponseData GetCloudInstances();
    }
}