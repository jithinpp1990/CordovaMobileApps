using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBankWebAPI.Helpers
{
    public static class ErrorCodes
    {
        public static string InvalidCredentials
        {
            get
            {
                return "Invalid User Name or Password";
            }
        }  
        
        public static string  LoginAttemptsExceeded
        {
            get
            {
                return "Login attempts exceeded. Try one day after.";
            }
        } 
        public static string InvalidMobileNumber
        {
            get
            {
                return "App is only accesible through registered Device.";
            }
        } 
        public static string InvalidCustomerId
        {
            get
            {
                return "Invalid customer id";
            }
        }
        public static string InvalidAgentCode
        {
            get
            {
                return "Invalid Agent Code";
            }
        }
        public static string InvalidEmailId
        {
            get
            {
                return "No registered email address found. Please contact the administrator.";
            }
        }
        public static string InvalidMobileNr
        {
            get
            {
                return "Invalid mobile number registered. Please contact the administrator.";
            }
        }
    }
}