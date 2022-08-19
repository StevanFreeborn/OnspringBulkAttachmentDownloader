using CommandDotNet;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var appRunner = new AppRunner<Program>();
        var appResult = await appRunner.RunAsync(args);
        return appResult;
    }

    [DefaultCommand]
    public async Task GetAttachments()
    {
        var apiKey = Prompt.GetApiKey();
        var source = Prompt.GetSource();

        if (source == Source.App)
        {
            var sourceId = Prompt.GetReportOrAppSource();
            var onspringService = new OnspringService(apiKey);
            
            var fieldIds = await onspringService.GetFilesForApp(sourceId);

            foreach (var id in fieldIds)
            {
                Console.WriteLine(id);
            }

        }

        if (source == Source.Report)
        {
            var sourceId = Prompt.GetReportOrAppSource();
        }

        if (source == Source.Records)
        {
            var sourceIds = Prompt.GetRecordsSource();
        }
    }
}