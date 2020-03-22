using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RD.SALV.CORE.Models
{
    /// <summary>
    /// Cloud instances connected with App insights
    /// </summary>
    public class CloudInstancesData
    {
        /// <summary>
        /// Instance name of cloud app
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// Unique id for instance
        /// </summary>
        public string CloudRole { get; set; }
    }
}