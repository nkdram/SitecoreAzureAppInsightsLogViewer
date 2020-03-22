using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RD.SALV.CORE.Models
{
    /// <summary>
    /// Log Request Data
    /// </summary>
    public class LogRequestData
    {
        [Required]
        public string LogType { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public string CloudInstance { get; set; }

        public string ExpressionType { set; get; }

        public string Message { get; set; }

        [Required]
        public int Limit { get; set; }

    }
}