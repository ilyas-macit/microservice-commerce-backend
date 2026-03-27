using LogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogService.Infrastructure.Persistence;

public class LogDbContext : DbContext
{
    public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
    {
    }

    public DbSet<LogEntry> Logs => Set<LogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LogEntry>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Level).IsRequired();
            entity.Property(x => x.Message).IsRequired();
            entity.Property(x => x.ServiceName).IsRequired();
            entity.Property(x => x.Timestamp).IsRequired();
        });
    }
}
