using LogService.Domain.Entities;
using LogService.Domain.Interfaces;
using LogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LogService.Infrastructure.Repositories;

public class LogRepository : ILogRepository
{
    private readonly LogDbContext _context;

    public LogRepository(LogDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(LogEntry entry)
    {
        _context.Logs.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<LogEntry>> GetByServiceAsync(string serviceName)
    {
        return await _context.Logs
            .AsNoTracking()
            .Where(x => x.ServiceName == serviceName)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogEntry>> GetByLevelAsync(string level)
    {
        return await _context.Logs
            .AsNoTracking()
            .Where(x => x.Level == level)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync();
    }
}
