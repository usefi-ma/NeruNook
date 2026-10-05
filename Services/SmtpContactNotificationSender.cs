using System.Globalization;
using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using NeruNook.Configuration;
using NeruNook.Data;

namespace NeruNook.Services;

public sealed class SmtpContactNotificationSender(
    IOptions<SmtpOptions> smtpOptions,
    IOptions<ContactNotificationOptions> notificationOptions,
    ILogger<SmtpContactNotificationSender> logger) : IContactNotificationSender
{
    private readonly SmtpOptions _smtp = smtpOptions.Value;
    private readonly ContactNotificationOptions _notifications = notificationOptions.Value;

    public async Task SendAsync(ContactInquiry inquiry, CancellationToken cancellationToken = default)
    {
        if (!_smtp.IsConfigured || string.IsNullOrWhiteSpace(_notifications.OwnerEmail))
        {
            var issue = _smtp.ConfigurationIssue
                ?? $"Missing configuration: {ContactNotificationOptions.SectionName}:OwnerEmail";

            logger.LogWarning(
                "Contact inquiry {InquiryId} was stored, but its notification was skipped. {ConfigurationIssue}",
                inquiry.Id,
                issue);
            return;
        }

        if (!MailboxAddress.TryParse(_smtp.FromEmail, out var configuredFrom))
        {
            logger.LogWarning(
                "Contact inquiry {InquiryId} was stored, but its notification was skipped because Smtp:FromEmail is invalid.",
                inquiry.Id);
            return;
        }

        if (!MailboxAddress.TryParse(_notifications.OwnerEmail, out var ownerAddress))
        {
            logger.LogWarning(
                "Contact inquiry {InquiryId} was stored, but its notification was skipped because ContactNotifications:OwnerEmail is invalid.",
                inquiry.Id);
            return;
        }

        if (!MailboxAddress.TryParse(inquiry.Email, out var customerAddress))
        {
            logger.LogWarning(
                "Contact inquiry {InquiryId} was stored, but its notification was skipped because the submitted Reply-To address is invalid.",
                inquiry.Id);
            return;
        }

        var customerName = inquiry.Name.Replace('\r', ' ').Replace('\n', ' ').Trim();
        var submittedUtc = inquiry.CreatedAtUtc.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture);
        var bodyBuilder = new BodyBuilder
        {
            TextBody = BuildTextBody(inquiry, submittedUtc),
            HtmlBody = BuildHtmlBody(inquiry, submittedUtc)
        };

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_smtp.FromName, configuredFrom.Address));
        message.To.Add(ownerAddress);
        message.ReplyTo.Add(new MailboxAddress(customerName, customerAddress.Address));
        message.Subject = $"New NeruNook inquiry #{inquiry.Id} from {customerName}";
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        var secureSocket = _smtp.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;
        try
        {
            await client.ConnectAsync(_smtp.Host, _smtp.Port, secureSocket, cancellationToken);
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);

            logger.LogInformation(
                "Contact notification for inquiry {InquiryId} was sent successfully.",
                inquiry.Id);
        }
        finally
        {
            if (client.IsConnected)
            {
                try
                {
                    await client.DisconnectAsync(true, CancellationToken.None);
                }
                catch (Exception exception)
                {
                    logger.LogWarning(
                        exception,
                        "SMTP disconnect failed after processing inquiry {InquiryId}.",
                        inquiry.Id);
                }
            }
        }
    }

    private static string BuildTextBody(ContactInquiry inquiry, string submittedUtc)
    {
        return $"""
                New NeruNook Contact Inquiry

                Inquiry ID: {inquiry.Id}
                Name: {inquiry.Name}
                Email: {inquiry.Email}
                Company: {DisplayValue(inquiry.Company)}
                Project Type: {DisplayValue(inquiry.ProjectType)}
                Budget: {DisplayValue(inquiry.Budget)}
                Submitted: {submittedUtc}

                Message:
                {inquiry.Message}

                Reply to this email to respond directly to {inquiry.Email}.
                """;
    }

    private static string BuildHtmlBody(ContactInquiry inquiry, string submittedUtc)
    {
        var email = WebUtility.HtmlEncode(inquiry.Email);
        var emailHref = WebUtility.HtmlEncode($"mailto:{inquiry.Email}");

        return $"""
                <!doctype html>
                <html lang="en">
                <body style="margin:0;padding:24px;background:#f5f2ea;color:#182219;font-family:Arial,sans-serif;line-height:1.55;">
                  <div style="max-width:680px;margin:0 auto;background:#ffffff;border:1px solid #ddd8cc;padding:32px;">
                    <p style="margin:0 0 8px;font-size:12px;letter-spacing:0.14em;text-transform:uppercase;color:#657065;">NeruNook</p>
                    <h1 style="margin:0 0 28px;font-size:26px;font-weight:500;">New Contact Inquiry</h1>
                    <table role="presentation" style="width:100%;border-collapse:collapse;font-size:15px;">
                      {HtmlRow("Inquiry ID", inquiry.Id.ToString(CultureInfo.InvariantCulture))}
                      {HtmlRow("Name", inquiry.Name)}
                      <tr><th scope="row" style="width:140px;padding:8px 16px 8px 0;border-bottom:1px solid #ece8df;text-align:left;vertical-align:top;font-weight:600;">Email</th><td style="padding:8px 0;border-bottom:1px solid #ece8df;"><a href="{emailHref}" style="color:#243b2b;">{email}</a></td></tr>
                      {HtmlRow("Company", DisplayValue(inquiry.Company))}
                      {HtmlRow("Project Type", DisplayValue(inquiry.ProjectType))}
                      {HtmlRow("Budget", DisplayValue(inquiry.Budget))}
                      {HtmlRow("Submitted", submittedUtc)}
                    </table>
                    <h2 style="margin:28px 0 8px;font-size:16px;font-weight:600;">Message</h2>
                    <div style="white-space:pre-wrap;font-size:15px;">{WebUtility.HtmlEncode(inquiry.Message)}</div>
                    <p style="margin:28px 0 0;color:#657065;font-size:13px;">Reply to this email to respond directly to {email}.</p>
                  </div>
                </body>
                </html>
                """;
    }

    private static string HtmlRow(string label, string value)
    {
        return $"<tr><th scope=\"row\" style=\"width:140px;padding:8px 16px 8px 0;border-bottom:1px solid #ece8df;text-align:left;vertical-align:top;font-weight:600;\">{WebUtility.HtmlEncode(label)}</th><td style=\"padding:8px 0;border-bottom:1px solid #ece8df;\">{WebUtility.HtmlEncode(value)}</td></tr>";
    }

    private static string DisplayValue(string value) =>
        string.IsNullOrWhiteSpace(value) ? "Not provided" : value;
}
