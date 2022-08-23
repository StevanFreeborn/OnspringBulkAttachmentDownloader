
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
var source = Prompt.GetSource();

var onspringService = new OnspringService(apiKey);
var fileFieldIds = await onspringService.GetFileFieldsForApp(appId);

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
    var sourceIds = Prompt.GetRecordIds();
}

Log.CloseAndFlush();