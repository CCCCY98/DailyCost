using DailyCost.Api.Models;
using DailyCost.Application.DTOs.Auth;
using DailyCost.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyCost.Api.Controllers;

[Route("api/v1/auth")]
public sealed class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<AuthResponseDto>.Ok(result.Data))
            : BadRequest(ApiResponse<AuthResponseDto>.Fail(result.Message ?? "注册失败"));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<AuthResponseDto>.Ok(result.Data))
            : BadRequest(ApiResponse<AuthResponseDto>.Fail(result.Message ?? "登录失败"));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthTokensDto>>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshAsync(request, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<AuthTokensDto>.Ok(result.Data))
            : BadRequest(ApiResponse<AuthTokensDto>.Fail(result.Message ?? "刷新失败"));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LogoutAsync(GetUserId(), request, cancellationToken);
        return result.Success ? Ok(ApiResponse.Ok(result.Data)) : BadRequest(ApiResponse.Fail(result.Message ?? "登出失败"));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ForgotPasswordAsync(request, cancellationToken);
        return result.Success ? Ok(ApiResponse.Ok(result.Data)) : BadRequest(ApiResponse.Fail(result.Message ?? "操作失败"));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ResetPasswordAsync(request, cancellationToken);
        return result.Success ? Ok(ApiResponse.Ok(result.Data)) : BadRequest(ApiResponse.Fail(result.Message ?? "操作失败"));
    }
}

