using LogService.Domain.Entities;
using LogService.Domain.Interfaces;
using MediatR;

namespace LogService.Application.Commands.CreateLog;

public class CreateLogCommandHandler : IRequestHandler<CreateLogCommand>
{
    private readonly ILogRepository _logRepository;

    public CreateLogCommandHandler(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task Handle(CreateLogCommand request, CancellationToken cancellationToken)
    {
        var entry = LogEntry.Create(request.Level, request.Message, request.ServiceName, request.Exception);
        await _logRepository.AddAsync(entry);
    }
}
