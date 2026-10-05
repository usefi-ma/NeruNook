namespace NeruNook.Configuration;

public sealed class ContactNotificationOptions
{
    public const string SectionName = "ContactNotifications";

    public string OwnerEmail { get; set; } = "";
}
