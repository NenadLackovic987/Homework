namespace Homework.Common.Contracts
{
    public interface ITraceable
    {
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedDateTime { get; set; }
    }
}
