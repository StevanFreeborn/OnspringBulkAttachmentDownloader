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
        Log.Information($"Saving File {FileId} for Field {FieldId} for Record {RecordId}...");

        try
        {
            Directory.CreateDirectory(outputDirectory);
            var filePath = GetFilePath(outputDirectory, RecordId, FieldId, FileId, FileInfo!.Name);
            var fileStream = System.IO.File.Create(filePath);
            await FileContent!.Stream.CopyToAsync(fileStream);
            await fileStream.DisposeAsync();

            Log.Information("succeeded.");
        }
        catch (Exception e)
        {
            Log.Error($"failed. ({e.Message})");
        }
    }

    private string GetFilePath(string outputDirectory, int recordId, int fieldId, int fileId, string name)
    {
        var fileName = $"{recordId}-{fieldId}-{fileId}-{name}";
        return Path.Combine(outputDirectory, fileName);
    }
}
