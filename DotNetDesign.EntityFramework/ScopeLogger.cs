using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Common.Logging;

namespace DotNetDesign.EntityFramework
{
    public class ScopeLogger : IDisposable
    {
        private readonly ILog _logger;
        private readonly string _methodName;

        public ScopeLogger(ILog logger)
        {
            _logger = logger;
            _methodName = new StackFrame(1).GetMethod().Name;
            _logger.Trace("Enter " + _methodName);
        }

        public void Dispose()
        {
            _logger.Trace("Exit " + _methodName);
        }
    }
}
