using FluentValidation.Results;
using GoldenRaspberryAwards.Core.Enum;
using GoldenRaspberryAwards.Core.Notifications;

namespace GoldenRaspberryAwards.Core.Interfaces.Notifications;

public interface INotifier
{
    IReadOnlyList<Notification> GetNotifications();
    bool HasNotification();
    void Handle(Notification notification);
    void Handle(string message);
    void Handle(string message, NotificationType type);
    void Handle(Exception exception);
    void HandleException(Exception exception);
    void NotifyValidationErrors(ValidationResult validationResult);
    void Clean();
}
