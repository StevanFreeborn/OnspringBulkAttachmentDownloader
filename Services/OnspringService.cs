using Onspring.API.SDK;
using Onspring.API.SDK.Models;
using Onspring.API.SDK.Enums;
using Serilog;

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

        var totalPages = 1;
        var pagingRequest = new PagingRequest(1, 50);
        var currentPage = pagingRequest.PageNumber;

        do
        {
            try
            {
                var response = await _client.GetFieldsForAppAsync(appId, pagingRequest);

                if (response.IsSuccessful is true)
                {
                    totalPages = response.Value.TotalPages;

                    var fields = response.Value.Items
                    .Where(field => field.Type == FieldType.Attachment || field.Type == FieldType.Image)
                    .Select(field => field.Id)
                    .ToList();

                    var numOfFields = fields.Count;

                    Log.Information("Successfully retrieved file field ids for App {appId}. (page {currentPage} of {totalPages} - {numOfFields} file fields found)", appId, currentPage, totalPages, numOfFields);

                    fieldIds.AddRange(fields);
                }
                else
                {
                    throw new ApplicationException($"Status Code: {response.StatusCode} - {response.Message}");
                }
            }
            catch (Exception e)
            {
                var message = e.Message;
                Log.Error("Failed to retrieve file field ids for App {appId}. ({message})", appId, message);
            }

            pagingRequest.PageNumber++;
            currentPage = pagingRequest.PageNumber;

        } while (currentPage <= totalPages);

        return fieldIds;
    }

    public async Task GetAppFiles(int appId, List<int> fileFieldIds, string outputDirectory)
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
            try
            {
                var response = await _client.GetRecordsForAppAsync(request);

                if (response.IsSuccessful is true)
                {
                    totalPages = response.Value.TotalPages;
                    var records = response.Value.Items;

                    Log.Information("Successfully retrieved records for App {appId}. (page {currentPage} of {totalPages})", appId, currentPage, totalPages);

                    await GetAndSaveFiles(records, outputDirectory);
                }
                else
                {
                    throw new ApplicationException($"Status Code: {response.StatusCode} - {response.Message}");
                }
            }
            catch (Exception e)
            {
                var messge = e.Message;
                Log.Error("Failed to retrieve records for App {appId}. (page {currentPage} of {totalPages} - {message})", appId, currentPage, totalPages);
            }

            request.PagingRequest.PageNumber++;
            currentPage = request.PagingRequest.PageNumber;

        } while (currentPage <= totalPages);
    }

    public async Task GetReportFiles(int appId, List<int> fileFieldIds, int reportId, string outputDirectory)
    {
        try
        {
            var reportResponse = await _client.GetReportAsync(reportId);

            if (reportResponse.IsSuccessful is true)
            {
                var allRecordIds = reportResponse.Value.Rows.Select(row => row.RecordId).ToList();

                var numOfRecords = allRecordIds.Count;

                Log.Information("Successfully retrieved record ids from Report {reportId}. ({numOfRecords} record ids found)", reportId, numOfRecords);

                var pageSize = 50;
                var currentPage = 0;
                var correctedPage = currentPage + 1;
                var totalPages = allRecordIds.Count / pageSize;

                while (currentPage < totalPages)
                {
                    var recordIds = allRecordIds.Skip(pageSize * currentPage).Take(pageSize).ToList();

                    var request = new GetRecordsRequest
                    {
                        AppId = appId,
                        RecordIds = recordIds,
                        FieldIds = fileFieldIds,
                        DataFormat = DataFormat.Raw,
                    };

                    try
                    {
                        var response = await _client.GetRecordsAsync(request);

                        if (response.IsSuccessful is true)
                        {
                            var records = response.Value.Items;

                            Log.Information("Successfully retrieved records for Report {reportId} succeeded. (page {correctedPage} of {totalPages})", reportId, correctedPage, totalPages);

                            await GetAndSaveFiles(records, outputDirectory);
                        }
                        else
                        {
                            throw new ApplicationException($"Status Code: {response.StatusCode} - {response.Message}");
                        }
                    }
                    catch (Exception e)
                    {
                        var message = e.Message;
                        Log.Error("Failed to retrieve records for Report {reportId}. (page {correctedPage} of {totalPages} - {message})", reportId, correctedPage, totalPages, message);
                    }

                    currentPage++;
                }
            }
            else
            {
                throw new ApplicationException($"Status Code: {reportResponse.StatusCode} - {reportResponse.Message}");
            }
        }
        catch (Exception e)
        {
            var message = e.Message;
            Log.Error("Failed to retrieve records for Report {reportId}. (message)", reportId, message);
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
                    var fileInfoResponse = await _client.GetFileInfoAsync(recordId, fieldId, id);
                    var fileResponse = await _client.GetFileAsync(recordId, fieldId, id);
                    try
                    {
                        if (fileInfoResponse.IsSuccessful is true && fileResponse.IsSuccessful is true)
                        {
                            var file = new File
                            {
                                RecordId = recordId,
                                FieldId = fieldId,
                                FileId = id,
                                FileInfo = fileInfoResponse.Value,
                                FileContent = fileResponse.Value,
                            };

                            await file.Save(outputDirectory);

                            Log.Information("Successfully saved File {id} for Field {fieldId} for Record {recordId}.", id, fieldId, recordId);
                        }
                        else
                        {
                            throw new ApplicationException($"File Info Status Code: {fileInfoResponse.StatusCode} - {fileInfoResponse.Message}, File Status Code: {fileResponse.StatusCode} - {fileResponse.Message}");
                        }
                    }
                    catch (Exception e)
                    {
                        var message = e.Message;
                        Log.Error($"Failed to save File {id} for Field {fieldId} for Record {recordId}. ({message})", id, fieldId, recordId, message);
                    }
                }
            }
        }
    }
}