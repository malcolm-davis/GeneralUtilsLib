namespace GeneralUtils
{
    /// <summary>
    /// StringUtils provides convenient methods for parsing numbers and booleans represented as a string.  These methods never throw an exception and provide a default value to return.
    /// </summary>
    public class StringUtils
    {
        /// <summary>
        /// Returns the int value for a string, never throws an exception, provides a default value option.
        /// </summary>
        /// <param name="value">The string value representing an integer.</param>
        /// <param name="defaultValue">The default value if the representing value is not a number.</param>
        public static int ParseInt(string value, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }
            int result = 0;
            if (!Int32.TryParse(value, out result))
            {
                return defaultValue;
            }
            return result;
        }


        /// <summary>
        /// Returns the boolean value for a string.  never throws an exception.
        /// Parses all forms of true and 1, and all forms of false and 0.
        /// Returns the false if the value is not true.
        /// </summary>
        /// <param name="value">The string value representing true or false, or 1==true or 0==false.</param>
        /// <returns>Instead of throughing an exception, returns false for null, blank, or non-true/false strings</returns>
        public static bool ParseBool(string value)
        {
            return ParseBool(value, false);
        }

        /// <summary>
        /// Returns the boolean value for a string.  never throws an exception.
        /// Parses all forms of true and 1, and all forms of false and 0.
        /// If neither value, returns the default value
        /// </summary>
        /// <param name="value">The string value representing true or false, or 1==true or 0==false</param>
        /// <param name="defaultValue">The default value if the representing value is true or false.</param>
        /// <returns></returns>
        public static bool ParseBool(string value, bool defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            if (value.Trim().ToLower().Equals("true")
                || value.Trim().ToLower().Equals("yes")
                || value.Trim().ToLower().Equals("1"))
            {
                return true;
            }

            if (value.Trim().ToLower().Equals("false") 
                || value.Trim().ToLower().Equals("no")
                || value.Trim().ToLower().Equals("0"))
            {
                return false;
            }

            return defaultValue;
        }

        /// <summary>
        /// Verify the value is a boolean.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (value.Trim().ToLower().Equals("true")
                || value.Trim().ToLower().Equals("yes")
                || value.Trim().ToLower().Equals("1"))
            {
                return true;
            }

            if (value.Trim().ToLower().Equals("false")
                || value.Trim().ToLower().Equals("no")
                || value.Trim().ToLower().Equals("0"))
            {
                return true;
            }

            return false;
        }
    }
}
