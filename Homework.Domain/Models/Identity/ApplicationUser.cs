using Microsoft.AspNetCore.Identity;
using Homework.Common.Contracts;

namespace Homework.Domain.Models.Identity
{
    public class ApplicationUser : IdentityUser, ITraceable
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? NickName { get; set; }
        public string? Position { get; set; }
        public bool NeedPasswordReset { get; set; }
        public bool Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedDateTime { get; set; }
    }
}
