using Serilog;

public static class LoggerHelpper
{
    public static void LogStart()
    {
        Log.Information("Onspring Bulk Attachment Downloader Started");
    }
}