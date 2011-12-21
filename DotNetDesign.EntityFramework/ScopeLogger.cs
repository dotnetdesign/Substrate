using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Common.Logging;

namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Disposable object that will write a trace message when created and when disposed.
    /// </summary>
    public class ScopeLogger : IDisposable
    {
        private readonly ILog _logger;
        private readonly string _methodName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeLogger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ScopeLogger(ILog logger)
        {
            _logger = logger;
            _methodName = new StackFrame(1).GetMethod().Name;
            _logger.Trace("Enter " + _methodName);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _logger.Trace("Exit " + _methodName);
        }
    }
}
