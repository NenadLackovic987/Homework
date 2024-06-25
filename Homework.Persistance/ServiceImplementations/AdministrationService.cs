using Microsoft.Extensions.Configuration;
using Homework.Application.Services;
using Homework.Domain.Models;
using Homework.Domain.Models.Identity;
using Homework.Persistence.Data;

namespace Homework.Persistence.ServiceImplementations
{
    public class AdministrationService : IAdministrationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationIdentityDbContext _context;
        public DbSet<ApplicationUser> _dbSetApplicationUser;
        public DbSet<ResetSession> _dbSetResetSession;

        public AdministrationService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _context = new ApplicationIdentityDbContext(new DbContextOptions<ApplicationIdentityDbContext>(), configuration);
            this._dbSetApplicationUser = this._context.Set<ApplicationUser>();
            this._dbSetResetSession = this._context.Set<ResetSession>();
            _userManager = userManager;
        }

        public virtual async Task<string> ConsumeUserSession(Guid sessionId)
        {
            var session = await _dbSetResetSession.FirstOrDefaultAsync(p => p.SessionId == sessionId && p.IsActive);

            if (session == null)
            {
                return null!;
            }
            else
            {
                session.IsActive = false;
                session.NewPasswordUpdated = true;

                var user = await _dbSetApplicationUser.FindAsync(session.UserId);
                user!.PasswordHash = session.NewPassword;

                Save();

                return user!.Email!;
            }
        }

        public virtual async Task<string> CreateResetSession(Guid sesssionId, string email)
        {
            var user = await _dbSetApplicationUser.FirstOrDefaultAsync(p => p.Email == email);

            if (user != null)
            {
                var newPass = Guid.NewGuid();
                var hashedPassword = _userManager.PasswordHasher.HashPassword(user, newPass.ToString());
                await _dbSetResetSession.AddAsync(new ResetSession()
                {
                    NewPassword = hashedPassword,
                    IsActive = true,
                    CreatedBy = user.Id,
                    CreatedDateTime = DateTime.UtcNow,
                    SessionId = sesssionId,
                    UserId = user.Id
                });

                Save();
                return await Task.FromResult(newPass.ToString());
            }

            throw new ArgumentException("User not exists in database.");
        }

        public void Save()
        {
            _context!.SaveChanges();
        }
    }
}
