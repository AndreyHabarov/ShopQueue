using ShopQueue.Application.Dto;

namespace ShopQueue.Application.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(string email, string password, string role, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
}