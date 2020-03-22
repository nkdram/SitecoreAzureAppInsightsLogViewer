using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.SALV.CORE.Models
{
    /// <summary>
    /// Log Data from Azure
    /// </summary>
    public class LogData
    {
        /// <summary>
        /// Log Message
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// Log Time Stamp
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Log Type 
        /// </summary>
        public LogType LogType { get; set; }
    }
}