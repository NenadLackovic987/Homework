using Homework.Common.Contracts;

namespace Homework.Web.Api.DataTransferObjects.Users
{
    public class UserApiResponse : ITraceable
    {
        public string Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string NickName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool NeedPasswordReset { get; set; }
        public bool Status { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool IsLocked { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedDateTime { get; set; }
    }
}
