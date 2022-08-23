using CommandDotNet;
using Onspring.API.SDK.Models;
using Onspring.API.SDK.Enums;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var appRunner = new AppRunner<Program>();
        var appResult = await appRunner.RunAsync(args);
        return appResult;
    }

    [DefaultCommand]
    public async Task GetAttachments(
        [Operand(Description = "The id for the app where the attachments are held.")]
        int appId,
        [Operand(Description = "Indicates whether the attachments will be collected from all records in an app, all records in a report, or specific records.")]
        string source)
    {
        source = source.ToLower();

        if (Prompt.IsValidSource(source) is false) return;

        var apiKey = Prompt.GetApiKey();
        var onspringService = new OnspringService(apiKey);
        var fileFieldIds = await onspringService.GetFileFieldsForApp(appId);

        if (source == Source.App)
        {
            await onspringService.GetAppFiles(appId, fileFieldIds);
        }

        if (source == Source.Report)
        {
            var reportId = Prompt.GetReportId();

            await onspringService.GetReportFiles(appId, fileFieldIds, reportId);
        }

        if (source == Source.Records)
        {
            var sourceIds = Prompt.GetRecordIds();
        }
    }
}