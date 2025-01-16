using GoldenRaspberryAwards.Core.Enum;

namespace GoldenRaspberryAwards.Core.Notifications;

public class Notification
{
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public Notification(string message, NotificationType type)
    {
        Message = message;
        Type = type;
    }
}
