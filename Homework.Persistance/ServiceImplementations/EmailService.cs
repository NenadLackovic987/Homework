using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Homework.Persistence.ServiceImplementations
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public Message(string to, string subject, string content)
        {
            To = new List<MailboxAddress>() { new MailboxAddress("test", to) };
            Subject = subject;
            Content = content;
        }
    }

    public class EmailService : Application.Services.IEmailService
    {
        public void SendResetPasswordEmail(string to, Guid sessionId, string password)
        {
            var url = $"http://localhost:51818/Login/ConfirmResetPassword?sessionId={sessionId}"; // to do: move to some configuration
            var message = CreateEmailMessage("ResetPassword", new Message(to, "ResetPassword", $"Dear {to}, please use this link to reset your password {url}. Your new pass: {password}"));
            Send(message);
        }

        private MimeMessage CreateEmailMessage(string subject, Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("admin", "homeworkprogramming2024@hotmail.com")); // to do: move to some secret store
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate("homeworkprogramming2024@hotmail.com", "W1llkommen");

                    client.Send(mailMessage);
                }
                catch (Exception ex) 
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
