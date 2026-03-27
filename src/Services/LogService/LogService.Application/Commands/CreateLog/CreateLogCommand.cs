using MediatR;

namespace LogService.Application.Commands.CreateLog;

public class CreateLogCommand : IRequest
{
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string? Exception { get; set; }
}
