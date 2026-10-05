namespace NeruNook.Configuration;

public sealed class SmtpOptions
{
    private const string DevelopmentPasswordPlaceholder = "REPLACE_WITH_REAL_PASSWORD";

    public const string SectionName = "Smtp";

    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromEmail { get; set; } = "";
    public string FromName { get; set; } = "NeruNook";

    public bool IsConfigured => ConfigurationIssue is null;

    public string? ConfigurationIssue
    {
        get
        {
            var missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(Host))
            {
                missingFields.Add($"{SectionName}:Host");
            }

            if (Port is <= 0 or > 65535)
            {
                missingFields.Add($"{SectionName}:Port");
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                missingFields.Add($"{SectionName}:Username");
            }

            if (string.IsNullOrWhiteSpace(Password)
                || string.Equals(Password, DevelopmentPasswordPlaceholder, StringComparison.Ordinal))
            {
                missingFields.Add($"{SectionName}:Password");
            }

            if (string.IsNullOrWhiteSpace(FromEmail))
            {
                missingFields.Add($"{SectionName}:FromEmail");
            }

            return missingFields.Count == 0
                ? null
                : $"Missing or placeholder configuration: {string.Join(", ", missingFields)}";
        }
    }
}
