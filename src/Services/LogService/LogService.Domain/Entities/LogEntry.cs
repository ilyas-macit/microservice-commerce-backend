namespace LogService.Domain.Entities;

public class LogEntry
{
    public Guid Id { get; private set; }
    public string Level { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string ServiceName { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }
    public string? Exception { get; private set; }

    public static LogEntry Create(string level, string message, string serviceName, string? exception = null)
    {
        return new LogEntry
        {
            Id = Guid.NewGuid(),
            Level = level,
            Message = message,
            ServiceName = serviceName,
            Timestamp = DateTime.UtcNow,
            Exception = exception
        };
    }
}
