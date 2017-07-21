using System;
using System.Diagnostics;

namespace Jeeves.Core
{
    [DebuggerStepThrough]
    public static class ValidationExtensions
    {
        internal static T ThrowIfNull<T>([ValidatedNotNull] this T t, string paramName)
            where T : class
        {
            if (t == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (paramName == null)
            {
                throw new ArgumentNullException(nameof(paramName));
            }

            return t;
        }
    }
}
