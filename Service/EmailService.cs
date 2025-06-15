using System.Net;
using System.Net.Mail;

//umhg dzsq mjxr tjyf


using Microsoft.Extensions.Configuration;

namespace Try.Service
{
    public class EmailService
    {
        private readonly string _from;
        private readonly string _smtp;
        private readonly int _port;
        private readonly string _password;

        public EmailService(IConfiguration configuration)
        {
            var settings = configuration.GetSection("EmailSettings");
            _from = settings["From"];
            _smtp = settings["Smtp"];
            _port = int.Parse(settings["Port"]);
            _password = settings["Password"];
        }

        public void Send(string to, string subject, string body)
        {
            var client = new SmtpClient(_smtp, _port)
            {
                Credentials = new NetworkCredential(_from, _password),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_from, "LaBelleCouture"),

                Subject = subject,

                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            mail.ReplyToList.Add(new MailAddress(_from));

            mail.Headers.Add("X-Priority", "1");
            mail.Headers.Add("X-MSMail-Priority", "High");

            client.Send(mail);
        }


    }
}
