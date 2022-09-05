using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GeneralUtils
{
    /// <summary>
    ///  A set of convenience methods to verify method arguments.
    /// </summary>
    public static class MethodUtils
    {
        /// <summary>
        /// A convenience method that ensures that the specified argument is not null.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="message">Optional additional message.</param>
        [System.Diagnostics.DebuggerStepThrough]
        public static void NotNull(object param, string paramName, [Optional, DefaultParameterValue("")] string message)
        {
            if (param == null)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentNullException(paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }

        /// <summary>
        /// A convenience method that ensures that the specified argument is not null, empty, or white spaces
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="message">Optional additional message.</param>
        [System.Diagnostics.DebuggerStepThrough]
        public static void NotNullOrEmpty(string param, string paramName, [Optional, DefaultParameterValue("")] string message)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentNullException(paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }
    }
}
