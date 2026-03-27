using LogService.Domain.Entities;

namespace LogService.Domain.Interfaces;

public interface ILogRepository
{
    Task AddAsync(LogEntry entry);
    Task<IEnumerable<LogEntry>> GetByServiceAsync(string serviceName);
    Task<IEnumerable<LogEntry>> GetByLevelAsync(string level);
}
