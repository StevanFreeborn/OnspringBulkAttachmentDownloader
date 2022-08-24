
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var outputDirectory = FileHelper.GetOutputDirectory();
var logPath = Path.Combine(outputDirectory, "log.json");

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.WriteTo.File(new RenderedCompactJsonFormatter(), logPath)
.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
.CreateLogger();

var apiKey = Prompt.GetApiKey();
var appId = Prompt.GetAppId();
var fileFieldIds = Prompt.GetFileFieldIds();
var source = Prompt.GetSource();

var onspringService = new OnspringService(apiKey);

if (source == Source.App)
{
    LoggerHelpper.LogStart();
    await onspringService.GetAppFiles(appId, fileFieldIds, outputDirectory);
}

if (source == Source.Report)
{
    var reportId = Prompt.GetReportId();
    LoggerHelpper.LogStart();
    await onspringService.GetReportFiles(appId, fileFieldIds, reportId, outputDirectory);
}

if (source == Source.Records)
{
    var recordIds = Prompt.GetRecordIds();
    LoggerHelpper.LogStart();
    await onspringService.GetRecordsFiles(appId, fileFieldIds, recordIds, outputDirectory);
}

Log.Information("Onspring Bulk Attachment Downloader Finished");

Log.CloseAndFlush();

Console.WriteLine("Presss any key to close...");
Console.ReadLine();