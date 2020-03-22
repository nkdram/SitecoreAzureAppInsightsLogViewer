using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RD.SALV.CORE.Models
{
    /// <summary>
    /// Log Types
    /// </summary>
    public enum LogType
    {
        //Verbose = 0,
        Information = 1,
        Warning = 2,
        Error = 3,
        //Critical = 4
    }

    /// <summary>
    /// Message query Expression
    /// </summary>
    public enum MessageExpresssion
    {
        [Description("startswith")]
        startswith = 0,
        [Description("endswith")]
        endswith = 1,
        [Description("contains")]
        contains = 2,
        [Description("!contains")]
        notcontains = 3
    }

}