using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetDesign.Substrate.WebApi
{
    /// <summary>
    /// Loggers for the DotNetDesign.Substrate.WebApi assembly.
    /// </summary>
    internal static class Logger
    {
        /// <summary>
        /// DotNetDesign.Substrate.WebApi assembly logger.
        /// </summary>
        internal static ILog Assembly = LogManager.GetLogger("DotNetDesign.Substrate.WebApi");
    }
}
