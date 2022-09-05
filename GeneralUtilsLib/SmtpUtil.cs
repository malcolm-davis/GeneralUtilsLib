namespace GeneralUtils
{
    public class SmtpUtil
    {
        public static bool ValidateAddress(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            if (email.Contains(","))
            {
                foreach (String toWhom in email.Split(","))
                {
                    if (!IsValidEmail(toWhom))
                    {
                        return false;
                    }

                }
                return true;
            }
            return IsValidEmail(email);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
