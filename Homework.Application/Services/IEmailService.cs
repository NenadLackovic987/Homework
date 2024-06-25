namespace Homework.Application.Services
{
    public interface IEmailService
    {
        public void SendResetPasswordEmail(string to, Guid sessionId, string password);
    }
}
