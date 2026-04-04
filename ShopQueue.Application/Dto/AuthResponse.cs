namespace ShopQueue.Application.Dto;

public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);