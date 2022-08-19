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
                    Console.WriteLine($"{source} is not a valid source. Please enter {Source.App}, {Source.Report}, or {Source.Records}");
                    source = null;
                }
            }
        }

        return source;
    }

    public static int GetReportOrAppSource()
    {
        var sourceId = 0;

        while (sourceId is 0)
        {
            Console.Write("Please enter your source's id: ");
            var sourceIdInput = Console.ReadLine();

            if (int.TryParse(sourceIdInput, out int result))
            {
                sourceId = result;
            }
            else
            {
                Console.WriteLine($"{sourceIdInput} is not a valid id.");
            }
        }

        return sourceId;
    }

    public static List<int> GetRecordsSource()
    {
        var ids = new List<int>();

        while(ids.Count < 1)
        {
            Console.Write("Please enter your source's ids: ");
            var idsInput = Console.ReadLine();

            if (!String.IsNullOrWhiteSpace(idsInput))
            {
                var idStrings = idsInput
                .Split(',', StringSplitOptions.TrimEntries)
                .ToList();

                foreach(var id in idStrings)
                {
                    var parsedId = 0;

                    if (!int.TryParse(id, out int result))
                    {
                        Console.WriteLine($"{id} is an invalid source id. Please try entering your source's ids again.");
                        ids.Clear();
                        break;
                    }

                    parsedId = result;
                    ids.Add(parsedId);
                }
            }
        }

        return ids;
    }
}