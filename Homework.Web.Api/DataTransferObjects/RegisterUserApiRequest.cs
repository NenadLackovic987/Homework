namespace Homework.Web.Api.DataTransferObjects
{
    public class RegisterUserApiRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? NickName { get; set; }
        public string Position { get; set; } = null!;
        public bool NeedPasswordReset { get; set; }
    }
}
