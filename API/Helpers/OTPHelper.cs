using System;

namespace CCBankWebAPI.Helpers
{
    public class OTPHelper
    {
        public string GenerateOTP(int length)
        {
            string sOTP = string.Empty;
            string sTempChars = string.Empty;
            Random rand = new Random();
            string[] sAllowedChars = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            for (int i = 0; i < length; i++)
            {
                int p = rand.Next(0, sAllowedChars.Length);
                sTempChars = sAllowedChars[rand.Next(0, sAllowedChars.Length)];
                sOTP += sTempChars;
            }
            return sOTP;
        }
    }
}