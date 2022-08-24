
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var apiKey = Prompt.GetApiKey();
var appId = Prompt.GetAppId();
var fileFieldIds = Prompt.GetFileFieldIds();
var source = Prompt.GetSource();

var outputDirectory = FileHelper.GetOutputDirectory();
var logPath = Path.Combine(outputDirectory, "log.json");

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.WriteTo.File(new RenderedCompactJsonFormatter(), logPath)
.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
.CreateLogger();

var onspringService = new OnspringService(apiKey);

if (source == Source.App)
{
    await onspringService.GetAppFiles(appId, fileFieldIds, outputDirectory);
}

if (source == Source.Report)
{
    var reportId = Prompt.GetReportId();
    await onspringService.GetReportFiles(appId, fileFieldIds, reportId, outputDirectory);
}

if (source == Source.Records)
{
    var recordIds = Prompt.GetRecordIds();
    await onspringService.GetRecordsFiles(appId, fileFieldIds, recordIds, outputDirectory);
}

Log.CloseAndFlush();