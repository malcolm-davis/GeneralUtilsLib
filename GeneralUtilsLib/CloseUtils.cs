namespace GeneralUtils
{
    public static class CloseUtils
    {
        /// <summary>
        /// Close stream.  Ignore null stream and exceptions
        /// </summary>
        /// <param name="stream">Stream to close.  Exceptions are ignored</param>
        public static Stream Dispose(Stream stream)
        {
            if (stream != null)
            {
                try
                {
                    stream.Dispose();
                }
                catch
                {
                    // ignore any issues.
                }
            }

            return stream;
        }

        /// <summary>
        /// Close stream.  Ignore null stream and exceptions
        /// </summary>
        /// <param name="stream">Stream to close.  Exceptions are ignored</param>
        public static IDisposable Dispose(IDisposable stream)
        {
            if (stream != null)
            {
                try
                {
                    stream.Dispose();
                }
                catch
                {
                    // ignore any issues.
                }
            }

            return stream;
        }
    }
}
