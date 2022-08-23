using Onspring.API.SDK;
using Onspring.API.SDK.Models;
using Onspring.API.SDK.Enums;

public class OnspringService
{
    private readonly string baseUrl = "https://api.onspring.com/";
    public readonly OnspringClient _client;

    public OnspringService(string apiKey)
    {
        _client = new OnspringClient(baseUrl, apiKey);
    }

    public async Task<List<int>> GetFileFieldsForApp(int appId)
    {
        var fieldIds = new List<int>();

        try
        {
            var totalPages = 1;
            var pagingRequest = new PagingRequest(1, 50);
            var currentPage = pagingRequest.PageNumber;

            do
            {
                Console.Write($"Getting file field ids for app {appId}...");

                var response = await _client.GetFieldsForAppAsync(appId, pagingRequest);

                if (response.IsSuccessful is true)
                {
                    totalPages = response.Value.TotalPages;

                    var fields = response.Value.Items
                    .Where(field => field.Type == FieldType.Attachment || field.Type == FieldType.Image)
                    .Select(field => field.Id)
                    .ToList();

                    Console.WriteLine($"succeeded. (page {currentPage} of {totalPages} - {fields.Count} file fields found)");

                    fieldIds.AddRange(fields);
                }
                else
                {
                    throw new ApplicationException($"Status Code: {response.StatusCode} - {response.Message})");
                }

                pagingRequest.PageNumber++;
                currentPage = pagingRequest.PageNumber;

            } while (currentPage <= totalPages);
        }
        catch (Exception e)
        {
            Console.WriteLine($"failed. ({e.Message})");
        }

        return fieldIds;
    }

    public async Task GetAppFiles(int appId, List<int> fileFieldIds, string outputDirectory)
    {
        try
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

                var response = await _client.GetRecordsForAppAsync(request);

                if (response.IsSuccessful is true)
                {
                    totalPages = response.Value.TotalPages;
                    var records = response.Value.Items;

                    Console.WriteLine($"succeeded. (page {currentPage} of {totalPages})");
                    
                    await GetAndSaveFiles(records, outputDirectory);
                }
                else
                {
                    throw new ApplicationException($"failed. (page {currentPage} of {totalPages} - Status Code: {response.StatusCode} - {response.Message})");
                }

                request.PagingRequest.PageNumber++;
                currentPage = request.PagingRequest.PageNumber;

            } while (currentPage <= totalPages);
        }
        catch (Exception e)
        {
            Console.WriteLine($"failed. ({e.Message})");
        }
    }

    public async Task GetReportFiles(int appId, List<int> fileFieldIds, int reportId, string outputDirectory)
    {
        try
        {
            Console.Write($"Getting record ids from Report {reportId}...");

            var reportResponse = await _client.GetReportAsync(reportId);

            if (reportResponse.IsSuccessful is true)
            {
                var allRecordIds = reportResponse.Value.Rows.Select(row => row.RecordId).ToList();

                Console.WriteLine($"succeeded. ({allRecordIds.Count} record ids found)");

                var pageSize = 50;
                var currentPage = 0;
                var totalPages = allRecordIds.Count / pageSize;

                while (currentPage < totalPages)
                {
                    var recordIds = allRecordIds.Skip(pageSize * currentPage).Take(pageSize).ToList();

                    Console.Write($"Getting records for Report {reportId}...");

                    var request = new GetRecordsRequest
                    {
                        AppId = appId,
                        RecordIds = recordIds,
                        FieldIds = fileFieldIds,
                        DataFormat = DataFormat.Raw,
                    };

                    var response = await _client.GetRecordsAsync(request);

                    if (response.IsSuccessful is true)
                    {
                        Console.WriteLine($"succeeded. (page {currentPage + 1} of {totalPages})");

                        var records = response.Value.Items;
                        await GetAndSaveFiles(records, outputDirectory);
                    }
                    else
                    {
                        throw new ApplicationException($"failed. (page {currentPage + 1} of {totalPages})");
                    }

                    currentPage++;
                }
            }
            else
            {
                throw new ApplicationException($"failed. (Status Code: {reportResponse.StatusCode} - {reportResponse.Message})");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"failed. ({e.Message})");
        }
    }

    private async Task GetAndSaveFiles(List<ResultRecord> records, string outputDirectory)
    {
        foreach (var record in records)
        {
            var recordId = record.RecordId;

            foreach (var field in record.FieldData)
            {
                var fieldId = field.FieldId;
                var fileIds = new List<int>();

                if (field.Type == ResultValueType.AttachmentList)
                {
                    fileIds.AddRange(field.AsAttachmentList().Select(file => file.FileId).ToList());
                }

                if (field.Type == ResultValueType.FileList)
                {
                    fileIds.AddRange(field.AsFileList());
                }

                foreach (var id in fileIds)
                {
                    Console.Write($"Getting File {id} for Field {fieldId} for Record {recordId}...");

                    var fileInfoResponse = await _client.GetFileInfoAsync(recordId, fieldId, id);
                    var fileResponse = await _client.GetFileAsync(recordId, fieldId, id);
                    try
                    {
                        if (fileInfoResponse.IsSuccessful is true && fileResponse.IsSuccessful is true)
                        {
                            Console.WriteLine("succeeded.");

                            var file = new File
                            {
                                RecordId = recordId,
                                FieldId = fieldId,
                                FileId = id,
                                FileInfo = fileInfoResponse.Value,
                                FileContent = fileResponse.Value,
                            };

                            await file.Save(outputDirectory);
                        }
                        else
                        {
                            throw new ApplicationException($"failed. (File Info Status Code: {fileInfoResponse.StatusCode} - {fileInfoResponse.Message}, File Status Code: {fileResponse.StatusCode} - {fileResponse.Message})");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"failed. ({e.Message})");
                    }
                }
            }
        }
    }
}