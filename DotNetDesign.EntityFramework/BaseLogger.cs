using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace DotNetDesign.EntityFramework
{
    public abstract class BaseLogger
    {
        /// <summary>
        /// Shared logger.
        /// </summary>
        protected readonly ILog Logger = Common.Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
