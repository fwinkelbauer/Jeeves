using System;

namespace Jeeves.Core
{
    public interface IJeevesLog
    {
        void Information(string messageTemplate, params object[] values);

        void Error(Exception ex, string messageTemplate, params object[] values);
    }
}
