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
        Console.Write($"Getting file field ids for app {appId}...");

        var pagingRequest = new PagingRequest(1, 50);

        var response = await _client.GetFieldsForAppAsync(appId, pagingRequest);

        if (response.IsSuccessful is false) return new List<int>();

        var fields = response.Value.Items;

        if (response.Value.TotalPages > 1)
        {
            var currentPage = response.Value.PageNumber;
            var totalPages = response.Value.TotalPages;

            while (currentPage != totalPages)
            {
                pagingRequest.PageNumber++;
                response = await _client.GetFieldsForAppAsync(appId, pagingRequest);

                foreach (var field in response.Value.Items)
                {
                    fields.Add(field);
                }

                currentPage = response.Value.PageNumber;
                totalPages = response.Value.TotalPages;
            }
        }

        var fieldIds = fields
        .Where(field => field.Type == FieldType.Attachment || field.Type == FieldType.Image)
        .Select(field => field.Id)
        .ToList();

        Console.WriteLine($"done. ({fieldIds.Count} file fields found)");

        return fieldIds;
    }

    public async Task<ApiResponse<GetPagedRecordsResponse>> GetAppRecords(GetRecordsByAppRequest request)
    {
        return await _client.GetRecordsForAppAsync(request);
    }

    public async Task<ApiResponse<ReportData>> GetReportRecords(int reportId)
    {
        return await _client.GetReportAsync(reportId);
    }

    public async Task GetAndSaveFiles(List<ResultRecord> records)
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

                        await file.Save();
                    }
                    else
                    {
                        Console.WriteLine($"failed.");
                    }
                }
            }
        }
    }
}