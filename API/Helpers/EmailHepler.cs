using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace CCBankWebAPI.Helpers
{
    public class EmailHepler
    {
        public bool Send(string to, string subject, string body)
        {
            var Mail = new MailMessage();
            var FromEmail = ConfigurationManager.AppSettings["Email"];
            var FromPassword = ConfigurationManager.AppSettings["Password"];
            Mail.To.Add(to);
            Mail.From = new MailAddress(FromEmail);
            Mail.Subject = subject;
            Mail.Body = body;
            Mail.IsBodyHtml = true;
            var client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(FromEmail, FromPassword);
            client.EnableSsl = true;
            try
            {
                client.Send(Mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}