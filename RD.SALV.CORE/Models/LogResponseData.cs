using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.SALV.CORE.Models
{
    /// <summary>
    /// Log response Data
    /// </summary>
    public class LogResponseData
    {
        public int NumberOfRows { get; set; }
        public bool ContainsErrors { get; set; }
        public List<LogData> Logs { get; set; }
    }
}