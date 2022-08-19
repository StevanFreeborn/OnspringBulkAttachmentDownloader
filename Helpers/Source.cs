public static class Source
{
    public static readonly string App = "app";
    public static readonly string Report = "report";
    public static readonly string Records = "records";

    public static bool IsValid(string source)
    {
        if (source == Source.App || 
            source == Source.Report || 
            source == Source.Records) return true;

        return false;
    }
}