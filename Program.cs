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
        var appId = Prompt.GetAppId();
        var source = Prompt.GetSource();

        var outputDirectory = FileHelper.GetOutputDirectory();
        var onspringService = new OnspringService(apiKey);
        var fileFieldIds = await onspringService.GetFileFieldsForApp(appId);

        if (source == Source.App)
        {
            await onspringService.GetAppFiles(appId, fileFieldIds, outputDirectory);
        }

        if (source == Source.Report)
        {
            var reportId = Prompt.GetReportId();

            await onspringService.GetReportFiles(appId, fileFieldIds, reportId, outputDirectory);
        }

        if (source == Source.Records)
        {
            var sourceIds = Prompt.GetRecordIds();
        }
    }
}