using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistDataAquisition.Helpers
{
    public static class EmailSender
    {
        public static void SendReport(List<string> mailAddresses, FileStream fileStream,string fileName)
        {            
            string username = ConfigurationManager.AppSettings["EmailUser"],
                password = ConfigurationManager.AppSettings["EmailPassword"],
                mailService = ConfigurationManager.AppSettings["EmailService"];
            int mailPort = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPort"]);

            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(mailService, mailPort);

            //SMTP Informations
            client.EnableSsl = true;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(username, password);

            mail.From = new System.Net.Mail.MailAddress(username);

            //Recipient
            mail.To.Clear();
            mail.To.Add(string.Join(",",mailAddresses));
            
            //Subject
            mail.Subject = string.Format("[DA] Distributor report - {0}",DateTime.Now.ToString("dd/MM/yyyy"));
            mail.Attachments.Add(new System.Net.Mail.Attachment(fileStream, fileName));

            client.Send(mail);
        }
    }
}
