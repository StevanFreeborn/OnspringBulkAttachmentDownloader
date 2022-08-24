using Onspring.API.SDK.Models;
using Serilog;

public class File
{
    public int RecordId { get; set; }
    public int FieldId { get; set; }
    public int FileId { get; set; }
    public GetFileInfoResponse? FileInfo { get; set; }
    public GetFileResponse? FileContent { get; set; }

    public async Task Save(string outputDirectory)
    {
        try
        {
            Directory.CreateDirectory(outputDirectory);
            var filePath = GetFilePath(outputDirectory, RecordId, FieldId, FileId, FileInfo!.Name);
            var fileStream = System.IO.File.Create(filePath);
            await FileContent!.Stream.CopyToAsync(fileStream);
            await fileStream.DisposeAsync();

            Log.Information("Successfully saved File {FileId} for Field {FieldId} for Record {RecordId}.", FileId, FieldId, RecordId);
        }
        catch (Exception e)
        {
            var message = e.Message;
            Log.Error("Failed to save File {FileId} for Field {FieldId} for Record {RecordId}. ({message})", FileId, FieldId, RecordId, message);
        }
    }

    private string GetFilePath(string outputDirectory, int recordId, int fieldId, int fileId, string name)
    {
        var fileName = $"{recordId}-{fieldId}-{fileId}-{name}";
        return Path.Combine(outputDirectory, fileName);
    }
}
