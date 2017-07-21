using System;

namespace Jeeves.Core
{
    public interface IJeevesLog
    {
        void DebugFormat(string format, params object[] values);

        void ErrorFormat(Exception ex, string format, params object[] values);
    }
}
