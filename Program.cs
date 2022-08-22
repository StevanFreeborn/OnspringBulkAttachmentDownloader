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
            var totalPages = 1;
            var pagingRequest = new PagingRequest(1, 50);

            var request = new GetRecordsByAppRequest
            {
                AppId = appId,
                FieldIds = fileFieldIds,
                DataFormat = DataFormat.Raw,
                PagingRequest = pagingRequest,
            };

            var currentPage = request.PagingRequest.PageNumber;

            do
            {
                Console.Write($"Getting records for {appId}...");

                var response = await onspringService.GetAppRecords(request);

                if (response.IsSuccessful is true)
                {
                    totalPages = response.Value.TotalPages;
                    var records = response.Value.Items;

                    Console.WriteLine($"succeeded. (page {currentPage} of {totalPages})");

                    await onspringService.GetAndSaveFiles(records);
                }
                else
                {
                    Console.WriteLine($"failed. (page {currentPage} of {totalPages})");
                }

                request.PagingRequest.PageNumber++;
                currentPage = request.PagingRequest.PageNumber;

            } while (currentPage <= totalPages);
        }

        if (source == Source.Report)
        {
            var reportId = Prompt.GetReportId();
            
            Console.WriteLine($"Getting records for Report {reportId}...");

            var response = onspringService.GetReportRecords(reportId);
        }

        if (source == Source.Records)
        {
            var sourceIds = Prompt.GetRecordIds();
        }
    }
}