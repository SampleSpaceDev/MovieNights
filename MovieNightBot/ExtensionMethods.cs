namespace MovieNightBot;

public static class ExtensionMethods
{
    public static long ToTimestamp(this DateTime dateTime)
    {
        TimeSpan timeSpan = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long) timeSpan.TotalSeconds;
    }
    
}