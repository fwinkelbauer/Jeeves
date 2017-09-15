using System;
using Jeeves.Core;
using Serilog;

namespace Jeeves
{
    internal class JeevesLog : IJeevesLog
    {
        private readonly ILogger _log = Log.ForContext("SourceContext", "Jeeves.Core");

        public void Information(string messageTemplate, params object[] values)
        {
            _log.Information(messageTemplate, values);
        }

        public void Error(Exception ex, string messageTemplate, params object[] values)
        {
            _log.Error(ex, messageTemplate, values);
        }
    }
}
