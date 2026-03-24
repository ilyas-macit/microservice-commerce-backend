using MediatR;

namespace AuthService.Application.Handlers;

/// <summary>
/// Kullanıcı ID'ye göre getirme komutu
/// </summary>
public class GetUserByIdQuery : IRequest<UserByIdQueryResponse>
{
    public int UserId { get; set; }

    public GetUserByIdQuery(int userId)
    {
        UserId = userId;
    }
}

public class UserByIdQueryResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserByIdQueryResponse>
{
    public Task<UserByIdQueryResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        // Implementation burada olacak
        throw new NotImplementedException();
    }
}
