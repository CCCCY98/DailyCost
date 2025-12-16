using DailyCost.Application.Common;
using DailyCost.Application.DTOs.Auth;

namespace DailyCost.Application.Services;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<Result<AuthResponseDto>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<Result<AuthTokensDto>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
    Task<Result<object>> LogoutAsync(Guid userId, RefreshTokenRequest request, CancellationToken cancellationToken);
    Task<Result<object>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken);
    Task<Result<object>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);
}

