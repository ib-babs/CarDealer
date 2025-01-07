using System.Net.Mail;
using System.Net;

namespace CarDealer.Service
{
    public static class EmailService
    {
        public static async void SendMessage(IConfiguration smtpConfig, MailMessage mailMessage, string recepient)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(smtpConfig["SmtpUsername"], smtpConfig["SmtpPassword"]),
                EnableSsl = true,
            };
            mailMessage.To.Add(recepient);
            await smtpClient.SendMailAsync(mailMessage);

        }
    }
}
