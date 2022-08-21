using Onspring.API.SDK.Models;

public class File
{
    public int RecordId { get; set; }
    public int FieldId { get; set; }
    public int FileId { get; set; }
    public GetFileInfoResponse? FileInfo { get; set; }
    public GetFileResponse? FileContent { get; set; }

    public async void Save()
    {
        var outputDirectory = GetOutputDirectory();
        Directory.CreateDirectory(outputDirectory);
        var filePath = GetFilePath(outputDirectory, RecordId, FieldId, FileId, FileInfo!.Name);
        var fileStream = System.IO.File.Create(filePath);
        await FileContent!.Stream.CopyToAsync(fileStream);
        await fileStream.DisposeAsync();
    }

    private string GetOutputDirectory()
    {
        var currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        var outputDirectory = $"{DateTime.Now.ToString("yyyyMMddHHmm")}-output";
        return Path.Combine(currentDirectory, outputDirectory); ;
    }

    private string GetFilePath(string outputDirectory, int recordId, int fieldId, int fileId, string name)
    {
        var fileName = $"{recordId}-{fieldId}-{fileId}-{name}";
        return Path.Combine(outputDirectory, fileName);
    }
}
