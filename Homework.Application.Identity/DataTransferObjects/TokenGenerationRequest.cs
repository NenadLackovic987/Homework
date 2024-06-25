namespace Homework.Application.Identity.DataTransferObjects
{
    public class TokenGenerationRequest
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? NickName { get; set; }
        public string Position { get; set; } = null!;
        public TimeSpan TokenLifeTime { get; set; }
        public TimeSpan RefreshTokenLifeTime { get; set; }
    }
}
