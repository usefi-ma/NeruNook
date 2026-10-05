using NeruNook.Data;

namespace NeruNook.Services;

public interface IContactNotificationSender
{
    Task SendAsync(ContactInquiry inquiry, CancellationToken cancellationToken = default);
}
