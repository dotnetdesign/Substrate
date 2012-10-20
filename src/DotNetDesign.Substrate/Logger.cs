using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetDesign.Substrate
{
    /// <summary>
    /// Loggers for the DotNetDesign.Substrate assembly.
    /// </summary>
    internal static class Logger
    {
        /// <summary>
        /// DotNetDesign.Substrate assembly logger.
        /// </summary>
        internal static ILog Assembly = LogManager.GetLogger("DotNetDesign.Substrate");
    }
}
