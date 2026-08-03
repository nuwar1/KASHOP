using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Common
{
    public class EmailSender: IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("nuwar0043@gmail.com", "oiok ofyu czik ljyt")
            };

            return client.SendMailAsync(
                new MailMessage(
                    from: "nuwar0043@gmail.com",
                    to: email,
                    subject,
                    message)
                { IsBodyHtml = true });
        }
    }
}
