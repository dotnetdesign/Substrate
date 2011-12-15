using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace DotNetDesign.EntityFramework
{
    public static class LoggerExtensions
    {
        public static IDisposable Scope(this ILog logger)
        {
            return new ScopeLogger(logger);
        }
    }
}
