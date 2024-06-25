namespace Homework.Application.Services
{
    public interface IAdministrationService
    {
        Task<string> ConsumeUserSession(Guid sessionId);
        Task<string> CreateResetSession(Guid sesssionId, string email);
        void Save();

    }
}
