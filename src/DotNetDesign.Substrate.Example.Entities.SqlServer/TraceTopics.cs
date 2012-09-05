using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetDesign.Substrate.Example.Entities.SqlServer
{

    internal static class TraceTopics
    {
        static TraceTopics()
        {
            SqlServer = LogManager.GetLogger("DotNetDesign.Substrate.Example.Entities.SqlServer");
        }

        internal static ILog SqlServer;
    }
}
