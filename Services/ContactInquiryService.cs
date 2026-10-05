using Microsoft.EntityFrameworkCore;
using NeruNook.Data;
using NeruNook.Models;

namespace NeruNook.Services;

public sealed class ContactInquiryService(
    IDbContextFactory<ApplicationDbContext> dbFactory,
    IContactNotificationSender notificationSender,
    ILogger<ContactInquiryService> logger)
{
    public async Task<ContactInquiry> SubmitAsync(
        ContactSubmissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var inquiry = new ContactInquiry
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Company = request.Company.Trim(),
            ProjectType = request.ProjectType.Trim(),
            Budget = request.Budget.Trim(),
            Message = request.Message.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            Status = ContactInquiryStatus.New,
            IsRead = false
        };

        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        db.ContactInquiries.Add(inquiry);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Contact inquiry {InquiryId} was saved to SQLite.",
            inquiry.Id);

        try
        {
            await notificationSender.SendAsync(inquiry, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Inquiry {InquiryId} was stored, but its owner notification could not be sent.",
                inquiry.Id);
        }

        return inquiry;
    }
}
