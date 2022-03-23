using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Revo.ServiceUtilities
{
    public class Notification
    {
        /// <summary>
        /// Sends mail for notify responsable person
        /// </summary>
        /// <param name="subject">mail title</param>
        /// <param name="body">mail content</param>
        /// <param name="from">sender mail address</param>
        /// <param name="to">receiver mail address</param>
        /// <param name="credentials">username and password</param>
        /// <param name="host">smtp server</param>
        /// <returns>success or error message</returns>
        public string SendMail(string subject, string body,string from,string to, NetworkCredential credentials,string host,int port,bool enableSsl) {

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(host);

            mail.From = new MailAddress(from);
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            SmtpServer.Port = port;
            SmtpServer.Credentials = credentials;
            SmtpServer.EnableSsl = enableSsl;

            try
            {
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "success";
        }
    }
}
