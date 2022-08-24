using Serilog;

public static class Prompt
{
    public static string GetApiKey()
    {
        string? apiKey = null;

        while (String.IsNullOrWhiteSpace(apiKey))
        {
            Console.Write("Please enter your api key: ");
            apiKey = Console.ReadLine();
        }

        return apiKey;
    }

    public static List<int> GetFileFieldIds()
    {
        var fileFieldIds = new List<int>();

        while (fileFieldIds.Count < 1)
        {
            Console.Write("Please enter your file field ids: ");
            var fileFieldIdsInput = Console.ReadLine();

            if (!String.IsNullOrWhiteSpace(fileFieldIdsInput))
            {
                var idStrings = fileFieldIdsInput
                .Split(',', StringSplitOptions.TrimEntries)
                .ToList();

                foreach (var id in idStrings)
                {
                    var parsedId = 0;

                    if (!int.TryParse(id, out int result))
                    {
                        Log.Error($"{id} is an invalid field id. Please try entering your file field ids again.");
                        fileFieldIds.Clear();
                        break;
                    }

                    parsedId = result;
                    fileFieldIds.Add(parsedId);
                }
            }
        }

        return fileFieldIds;
    }

    public static string GetSource()
    {
        string? source = null;

        while (String.IsNullOrWhiteSpace(source))
        {
            Console.Write("Please enter your source: ");
            source = Console.ReadLine();

            if (!String.IsNullOrWhiteSpace(source))
            {
                source = source.ToLower();

                if (!Source.IsValid(source))
                {
                    Log.Error($"{source} is not a valid source. Please enter one of the following options: {Source.App}, {Source.Report}, or {Source.Records}");
                    source = null;
                }
            }
        }

        return source;
    }

    public static int GetAppId()
    {
        var appId = 0;

        while (appId is 0)
        {
            Console.Write("Please enter your app id: ");
            var appIdInput = Console.ReadLine();

            if (int.TryParse(appIdInput, out int result))
            {
                appId = result;
            }
            else
            {
                Log.Error($"{appIdInput} is not a valid report id. Please try entering your app id again.");
            }
        }

        return appId;
    }

    public static int GetReportId()
    {
        var reportId = 0;

        while (reportId is 0)
        {
            Console.Write("Please enter your report's id: ");
            var reportIdInput = Console.ReadLine();

            if (int.TryParse(reportIdInput, out int result))
            {
                reportId = result;
            }
            else
            {
                Log.Error($"{reportIdInput} is not a valid report id. Please try entering your report id again");
            }
        }

        return reportId;
    }

    public static List<int> GetRecordIds()
    {
        var recordIds = new List<int>();

        while (recordIds.Count < 1)
        {
            Console.Write("Please enter your record ids: ");
            var recordIdsInput = Console.ReadLine();

            if (!String.IsNullOrWhiteSpace(recordIdsInput))
            {
                var idStrings = recordIdsInput
                .Split(',', StringSplitOptions.TrimEntries)
                .ToList();

                foreach (var id in idStrings)
                {
                    var parsedId = 0;

                    if (!int.TryParse(id, out int result))
                    {
                        Log.Error($"{id} is an invalid record id. Please try entering your record ids again.");
                        recordIds.Clear();
                        break;
                    }

                    parsedId = result;
                    recordIds.Add(parsedId);
                }
            }
        }

        return recordIds;
    }
}