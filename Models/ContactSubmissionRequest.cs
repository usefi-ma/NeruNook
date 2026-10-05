using System.ComponentModel.DataAnnotations;

namespace NeruNook.Models;

public sealed class ContactSubmissionRequest
{
    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = "";

    [StringLength(160)]
    public string Company { get; set; } = "";

    [StringLength(80)]
    public string ProjectType { get; set; } = "";

    [StringLength(80)]
    public string Budget { get; set; } = "";

    [Required, StringLength(4000, MinimumLength = 10)]
    public string Message { get; set; } = "";

    // Humans never see or fill this field. Bots commonly do.
    public string Website { get; set; } = "";
}
