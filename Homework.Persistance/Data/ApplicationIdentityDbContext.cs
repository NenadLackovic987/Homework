using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Homework.Common;
using Homework.Domain.Models;
using Homework.Domain.Models.Identity;
using System.Security.Claims;

namespace Homework.Persistence.Data
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ResetSession> ResetSessions { get; set; }

        private readonly IConfiguration _configuration;

        public ApplicationIdentityDbContext()
        {
        }

        public ApplicationIdentityDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public override int SaveChanges()
        {

            return base.SaveChanges();
        }

        public string GetCurrentUser()
        {
            var user = new HttpContextAccessor().HttpContext?.User;

            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
                return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            else
                return "NotAuthorizedUser";
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (_configuration is null)
            {
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS; Initial Catalog=IdentityDatabase; Integrated Security=True; TrustServerCertificate=True;",
                    b => b.MigrationsAssembly("Homework.Persistence"));
            }
            else
            {
                var connectionString = _configuration.GetConnectionString(Constants.DB_CONN_IDENTITY_NAME);
                optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("Homework.Persistence"));
            }

            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser() 
            {
                Id = "1126de3d-1d6a-45e1-9b55-75e8f46e9aaf",
                UserName = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAED2/qzQmy0ll1E6m1SifCvb67lXqJ3odcCrDt7eIeiyLM1LPZS2L0PtQLNl2fvcy1w==",
                SecurityStamp = "FT4ECTAV3EA4HMUJGBEJHNR3B5IL3XJ7",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000000",
                LockoutEnabled = true,
                AccessFailedCount = 0,
                FirstName = "Admin",
                LastName = "Admin",
                NickName = "Aki",
                Position = "Developer",
                NeedPasswordReset = false,
                Status = true,
                CreatedBy = "SystemUser",
                CreatedDateTime = new DateTimeOffset(year: 2023, month: 10, day: 1, hour: 4, minute: 0, second: 0, new TimeSpan())
            });
        }
    }
}
