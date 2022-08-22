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
                    Console.WriteLine($"{source} is not a valid source. Please pass one of the following options: {Source.App}, {Source.Report}, or {Source.Records}");
                    source = null;
                }
            }
        }

        return source;
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
                Console.WriteLine($"{reportIdInput} is not a valid report id.");
            }
        }

        return reportId;
    }

    public static List<int> GetRecordIds()
    {
        var recordIds = new List<int>();

        while(recordIds.Count < 1)
        {
            Console.Write("Please enter your record ids: ");
            var recordIdsInput = Console.ReadLine();

            if (!String.IsNullOrWhiteSpace(recordIdsInput))
            {
                var idStrings = recordIdsInput
                .Split(',', StringSplitOptions.TrimEntries)
                .ToList();

                foreach(var id in idStrings)
                {
                    var parsedId = 0;

                    if (!int.TryParse(id, out int result))
                    {
                        Console.WriteLine($"{id} is an invalid record id. Please try entering your record ids again.");
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

    public static bool IsValidSource(string source)
    {
        try
        {
            if (!Source.IsValid(source))
            {
                throw new ApplicationException($"{source} is not a valid source. Please pass one of the following for the source argument: {Source.App}, {Source.Report}, or {Source.Records}");
            }
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}