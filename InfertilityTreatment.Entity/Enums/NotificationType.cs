namespace InfertilityTreatment.Entity.Enums
{
    public enum NotificationType : byte
    {
        General = 1,
        Announcement = 2,
        Emergency = 3,
        Maintenance = 4,
        Reminder = 5,
        Appointment = 6,
        Result = 7,
        Alert = 8
    }

    public enum NotificationPriority : byte
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }
}
