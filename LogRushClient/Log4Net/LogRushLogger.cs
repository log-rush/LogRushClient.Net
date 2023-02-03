using log4net.Appender;
using log4net.Core;

namespace LogRushClient.Log4Net;

public class LogRushLogger : AppenderSkeleton
{
    protected override void Append(LoggingEvent loggingEvent)
    {
        Task.Run(async () =>
        {
            _logger ??= new Core.LogRushLogger(Alias, Server, Id, Key);
            await _logger.Log(RenderLoggingEvent(loggingEvent));
        });
    }
        
    public string Server { set; get; } = "";
    public string Alias { set; get; } = "";
    public string Id { set; get; } = "";
    public string Key { set; get; } = "";
        
    private Core.LogRushLogger? _logger;
}