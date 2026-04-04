using Microsoft.AspNetCore.Mvc;
using ShopQueue.Application.Services;
using ShopQueue.Domain.Enums;
using ShopQueue.Requests;

namespace ShopQueue.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result =
            await authService.RegisterAsync(request.Email, request.Password, UserRole.Customer, cancellationToken);
        return Ok(result);
    }

    [HttpPost("register-owner")]
    public async Task<IActionResult> RegisterOwner([FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await authService.RegisterAsync(request.Email, request.Password, UserRole.Owner, cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request.Email, request.Password, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshAsync(request.RefreshToken, cancellationToken);
        return Ok(result);
    }
}