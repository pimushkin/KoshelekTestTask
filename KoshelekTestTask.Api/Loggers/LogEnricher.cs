using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Events;

namespace KoshelekTestTask.Api.Loggers
{
    class LogEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent le, ILogEventPropertyFactory lepf)
        {
            le.RemovePropertyIfPresent("SourceContext");
            le.RemovePropertyIfPresent("RequestId");
            le.RemovePropertyIfPresent("RequestPath");
            le.RemovePropertyIfPresent("ActionId");
            le.RemovePropertyIfPresent("ActionName");

            le.AddPropertyIfAbsent(lepf.CreateProperty("MachineName", Environment.MachineName));
        }
    }
}
