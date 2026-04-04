using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopQueue.Application.Dto;
using ShopQueue.Application.Exceptions;
using ShopQueue.Application.Repositories;
using ShopQueue.Application.Services;
using ShopQueue.Domain.Entities;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ShopQueue.Infrastructure.Services;

public class AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork, IConfiguration configuration)
    : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(string email, string password, string role,
        CancellationToken cancellationToken = default)
    {
        var existing = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (existing != null)
            throw new BusinessException("User with this email already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        SetRefreshToken(user);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new BusinessException("Invalid email or password");

        SetRefreshToken(user);

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
        if (user is null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
            throw new BusinessException("Invalid or expired refresh token");

        SetRefreshToken(user);

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return GenerateAuthResponse(user);
    }

    private void SetRefreshToken(User user)
    {
        user.RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(
            configuration.GetValue<int>("Jwt:RefreshExpiresInDays"));
    }

    private AuthResponse GenerateAuthResponse(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(
            configuration.GetValue<int>("Jwt:ExpiresInMinutes"));

        var token = GenerateAccessToken(user, expiresAt);

        return new AuthResponse(token, user.RefreshToken!, expiresAt);
    }

    private string GenerateAccessToken(User user, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}