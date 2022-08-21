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

    public async Task<List<File>> GetFiles(List<ResultRecord> records)
    {
        var files = new List<File>();

        foreach (var record in records)
        {
            var recordId = record.RecordId;

            foreach (var field in record.FieldData)
            {
                var fieldId = field.FieldId;

                if (field.Type == ResultValueType.AttachmentList)
                {
                    foreach (var file in field.AsAttachmentList())
                    {
                        var attachmentFileInfoResponse = await _client.GetFileInfoAsync(recordId, fieldId, file.FileId);
                        var attachmentFileResponse = await _client.GetFileAsync(recordId, fieldId, file.FileId);

                        if (attachmentFileInfoResponse.IsSuccessful is true && attachmentFileResponse.IsSuccessful is true)
                        {
                            files.Add(new File
                            {
                                RecordId = recordId,
                                FieldId = fieldId,
                                FileId = file.FileId,
                                FileInfo = attachmentFileInfoResponse.Value,
                                FileContent = attachmentFileResponse.Value,
                            });
                        }
                    }
                }

                if (field.Type == ResultValueType.FileList)
                {
                    foreach (var id in field.AsFileList())
                    {
                        var imageFileInfoResponse = await _client.GetFileInfoAsync(recordId, fieldId, id);
                        var imageFileResponse = await _client.GetFileAsync(recordId, fieldId, id);

                        if (imageFileInfoResponse.IsSuccessful is true && imageFileResponse.IsSuccessful is true)
                        {
                            files.Add(new File
                            {
                                RecordId = recordId,
                                FieldId = fieldId,
                                FileId = id,
                                FileInfo = imageFileInfoResponse.Value,
                                FileContent = imageFileResponse.Value,
                            });
                        }
                    }
                }
            }
        }

        return files;
    }
}