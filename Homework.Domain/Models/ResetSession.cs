using Homework.Common.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homework.Domain.Models
{
    [Table(nameof(ResetSession))]
    public class ResetSession : ITraceable
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid SessionId { get; set; }
        public bool IsActive { get; set; }
        public string? NewPassword { get; set; }
        public bool NewPasswordUpdated { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedDateTime { get; set; }
    }
}
