namespace WebApp.Helpers;

public static class VietnamTimeHelper
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(7);

    public static DateTime? ToVietnamTime(this DateTime? utcDateTime)
    {
        if (utcDateTime == null)
            return null;

        return DateTime.SpecifyKind(utcDateTime.Value, DateTimeKind.Utc).Add(Offset);
    }

    public static DateTime ToVietnamTime(this DateTime utcDateTime)
    {
        return DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc).Add(Offset);
    }
}
