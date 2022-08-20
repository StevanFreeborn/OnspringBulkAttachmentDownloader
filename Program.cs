using CommandDotNet;
using Onspring.API.SDK.Models;
using Onspring.API.SDK.Enums;
using System.Text.Json;

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
        [Operand(Description = "Indicates whether the attachments will be collected from all records in an app, all records in a report, or specific record ids.")]
        string source)
    {
        source = source.ToLower();

        try
        {
            if (!Source.IsValid(source))
            {
                throw new ApplicationException($"{source} is not a valid source. Please pass one of the following for the source option: {Source.App}, {Source.Report}, or {Source.Records}");

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
        }

        var apiKey = Prompt.GetApiKey();
        var onspringService = new OnspringService(apiKey);
        var fileFieldIds = await onspringService.GetFileFieldsForApp(appId);

        if (source == Source.App)
        {
            var pagingRequest = new PagingRequest(1, 50);

            var request = new GetRecordsByAppRequest
            {
                AppId = appId,
                FieldIds = fileFieldIds,
                DataFormat = DataFormat.Raw,
                PagingRequest = pagingRequest,
            };

            var response = await onspringService._client.GetRecordsForAppAsync(request);

            if (response.IsSuccessful is false) return;

            var records = response.Value.Items;
                        
            foreach (var record in records)
            {
                var recordId = record.RecordId;

                foreach (var field in record.FieldData)
                {
                    var fieldId = field.FieldId;
                    var currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                    var outputDirectory = $"{DateTime.Now.ToString("yyyyMMddHHmm")}-output";
                    var outputDirectoryPath = Path.Combine(currentDirectory, outputDirectory);
                    Directory.CreateDirectory(outputDirectoryPath);

                    if (field.Type == ResultValueType.AttachmentList)
                    {
                        foreach (var file in field.AsAttachmentList())
                        {
                            var attachmentFileId = file.FileId;
                            var attachmentFileInfoResponse = await onspringService._client.GetFileInfoAsync(recordId, fieldId, attachmentFileId);
                            var attachmentFileResponse = await onspringService._client.GetFileAsync(recordId, fieldId, attachmentFileId);
                            
                            if (attachmentFileInfoResponse.IsSuccessful is true && attachmentFileResponse.IsSuccessful is true)
                            {
                                var attachmentFileInfo = attachmentFileInfoResponse.Value;
                                var attachmentFile = attachmentFileResponse.Value;
                                var attachmentFileName = $"{recordId}-{fieldId}-{attachmentFileId}-{attachmentFileInfo.Name}";
                                var attachmentFilePath = Path.Combine(currentDirectory, outputDirectory, attachmentFileName);
                                var attachmentFileStream = new FileStream(attachmentFilePath, FileMode.Create, FileAccess.Write);
                                await attachmentFile.Stream.CopyToAsync(attachmentFileStream);
                                await attachmentFileStream.DisposeAsync();
                            }
                        }
                    }

                    if (field.Type == ResultValueType.FileList)
                    {
                        foreach (var id in field.AsFileList())
                        {
                            var imageFileId = id;
                            var imageFileInfoResponse = await onspringService._client.GetFileInfoAsync(recordId, fieldId, imageFileId);
                            var imageFileResponse = await onspringService._client.GetFileAsync(recordId, fieldId, imageFileId);
                            
                            if (imageFileInfoResponse.IsSuccessful is true && imageFileResponse.IsSuccessful is true)
                            {
                                var imageFileInfo = imageFileInfoResponse.Value;
                                var imageFile = imageFileResponse.Value;
                                var attachmentFileName = $"{recordId}-{fieldId}-{imageFileId}-{imageFileInfo.Name}";
                                var imageFilePath = Path.Combine(currentDirectory, outputDirectory, attachmentFileName);
                                var imageFileStream = new FileStream(imageFilePath, FileMode.Create, FileAccess.Write);
                                await imageFile.Stream.CopyToAsync(imageFileStream);
                                await imageFileStream.DisposeAsync();
                            }
                        }
                    }

                }
            }
        }

        if (source == Source.Report)
        {
            var sourceId = Prompt.GetReportSource();
        }

        if (source == Source.Records)
        {
            var sourceIds = Prompt.GetRecordsSource();
        }
    }
}