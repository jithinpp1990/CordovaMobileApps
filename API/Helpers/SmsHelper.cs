using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace CCBankWebAPI.Helpers
{
    public class SmsHelper
    {
        public bool SendSms(string phone, string message)
        {
            var user_name = ConfigurationManager.AppSettings["Sms_Username"];
            var password = ConfigurationManager.AppSettings["Sms_Password"];
            var sender_id = ConfigurationManager.AppSettings["Sms_sender_id"];
            var sms_provider = ConfigurationManager.AppSettings["sms_provider"];
            Boolean status = true;
            try
            {
                switch (sms_provider)
                {
                    case "green ads":
                        {
                            int l_index;
                            var Is_url = string.Format("http://sapteleservices.in/SMS_API/sendsms.php?username={0}&password={1}&mobile={2}&sendername={3}&message={4}&routetype=1",
                                user_name, password, phone, sender_id, message);
                            var myrequest = (HttpWebRequest)WebRequest.Create(Is_url);
                            myrequest.Credentials = CredentialCache.DefaultCredentials;
                            var webResponse = (HttpWebResponse)myrequest.GetResponse();
                            var reader = new StreamReader(webResponse.GetResponseStream());
                            string str = reader.ReadLine();
                            l_index = str.LastIndexOf(':') + 2;
                            var sl_message_id = str.Substring(l_index, (str.Length - l_index));
                            status = true;
                            break;                           
                        }
                    default:
                        {
                            status = false;
                            break;
                        }
                }
                return status; ;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}