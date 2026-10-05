using System.ComponentModel.DataAnnotations;

namespace NeruNook.Data;

public enum ContactInquiryStatus
{
    New,
    InProgress,
    Replied,
    Archived
}

public sealed class ContactInquiry
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = "";

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = "";

    [MaxLength(160)]
    public string Company { get; set; } = "";

    [MaxLength(80)]
    public string ProjectType { get; set; } = "";

    [MaxLength(80)]
    public string Budget { get; set; } = "";

    [Required, MaxLength(4000)]
    public string Message { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ContactInquiryStatus Status { get; set; } = ContactInquiryStatus.New;

    public bool IsRead { get; set; }
}
