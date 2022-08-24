
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var apiKey = Prompt.GetApiKey();
var appId = Prompt.GetAppId();
var source = Prompt.GetSource();

var outputDirectory = FileHelper.GetOutputDirectory();
var logPath = Path.Combine(outputDirectory, "log.json");

Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.WriteTo.File(new RenderedCompactJsonFormatter(), logPath)
.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
.CreateLogger();

var onspringService = new OnspringService(apiKey);
var fileFieldIds = new List<int>();

if (source == Source.App)
{
    fileFieldIds = await onspringService.GetFileFieldsForApp(appId);
    await onspringService.GetAppFiles(appId, fileFieldIds, outputDirectory);
}

if (source == Source.Report)
{
    var reportId = Prompt.GetReportId();
    fileFieldIds = await onspringService.GetFileFieldsForApp(appId);

    await onspringService.GetReportFiles(appId, fileFieldIds, reportId, outputDirectory);
}

if (source == Source.Records)
{
    var sourceIds = Prompt.GetRecordIds();
    fileFieldIds = await onspringService.GetFileFieldsForApp(appId);
}

Log.CloseAndFlush();