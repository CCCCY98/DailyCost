using System.Security.Claims;
using DailyCost.Application.Abstractions;
using DailyCost.Application.Common;
using DailyCost.Application.DTOs.Auth;
using DailyCost.Application.DTOs.User;
using DailyCost.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DailyCost.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IAppDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthService(IAppDbContext db, IJwtService jwtService, IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _jwtService = jwtService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var exists = await _db.Users.AnyAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
        if (exists) return Result<AuthResponseDto>.Fail("该邮箱已注册");

        var now = _dateTimeProvider.UtcNow;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Nickname = request.Nickname,
            CreatedAt = now,
            UpdatedAt = now
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        var tokens = await CreateAndPersistTokensAsync(user, cancellationToken);
        var userDto = new UserDto(user.Id, user.Email, user.Nickname, user.Avatar, user.DefaultCalcMode, user.Currency, user.Timezone, user.CreatedAt, user.UpdatedAt, user.LastLoginAt);
        return Result<AuthResponseDto>.Ok(new AuthResponseDto(tokens, userDto));
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<AuthResponseDto>.Fail("邮箱或密码错误");

        if (!VerifyPassword(user, request.Password)) return Result<AuthResponseDto>.Fail("邮箱或密码错误");

        user.LastLoginAt = _dateTimeProvider.UtcNow;
        user.UpdatedAt = _dateTimeProvider.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        var tokens = await CreateAndPersistTokensAsync(user, cancellationToken);
        var userDto = new UserDto(user.Id, user.Email, user.Nickname, user.Avatar, user.DefaultCalcMode, user.Currency, user.Timezone, user.CreatedAt, user.UpdatedAt, user.LastLoginAt);
        return Result<AuthResponseDto>.Ok(new AuthResponseDto(tokens, userDto));
    }

    public async Task<Result<AuthTokensDto>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var token = request.RefreshToken.Trim();
        var now = _dateTimeProvider.UtcNow;

        var stored = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

        if (stored is null || stored.RevokedAt is not null || stored.ExpiresAt <= now || stored.User.IsDeleted)
            return Result<AuthTokensDto>.Fail("刷新令牌无效或已过期");

        stored.RevokedAt = now;
        await _db.SaveChangesAsync(cancellationToken);

        var tokens = await CreateAndPersistTokensAsync(stored.User, cancellationToken);
        return Result<AuthTokensDto>.Ok(tokens);
    }

    public async Task<Result<object>> LogoutAsync(Guid userId, RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var token = request.RefreshToken.Trim();
        var now = _dateTimeProvider.UtcNow;

        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token && rt.UserId == userId, cancellationToken);
        if (stored is not null && stored.RevokedAt is null)
        {
            stored.RevokedAt = now;
            await _db.SaveChangesAsync(cancellationToken);
        }

        return Result<object>.Ok(new { });
    }

    public async Task<Result<object>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
        if (user is null)
            return Result<object>.Ok(new { });

        var token = _jwtService.CreatePasswordResetToken(user.Id, user.Email);

        return Result<object>.Ok(new
        {
            resetToken = token,
            note = "演示实现：生产环境应通过邮件/短信发送重置链接，而不是直接返回 token。"
        });
    }

    public async Task<Result<object>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var principal = _jwtService.ValidatePasswordResetToken(request.Token);
        if (principal is null) return Result<object>.Fail("重置令牌无效或已过期");

        var sub = principal.FindFirstValue("sub") ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(sub, out var userId)) return Result<object>.Fail("重置令牌无效或已过期");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<object>.Fail("用户不存在");

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        user.UpdatedAt = _dateTimeProvider.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result<object>.Ok(new { });
    }

    private async Task<AuthTokensDto> CreateAndPersistTokensAsync(User user, CancellationToken cancellationToken)
    {
        var (accessToken, expiresAtUtc) = _jwtService.CreateAccessToken(user.Id, user.Email);
        var refreshToken = _jwtService.CreateRefreshToken();

        var now = _dateTimeProvider.UtcNow;
        var refresh = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            CreatedAt = now,
            ExpiresAt = now.AddDays(30)
        };

        _db.RefreshTokens.Add(refresh);
        await _db.SaveChangesAsync(cancellationToken);

        return new AuthTokensDto(accessToken, refreshToken, expiresAtUtc);
    }

    private bool VerifyPassword(User user, string password)
    {
        try
        {
            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return verify != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            // 旧数据可能不是 Identity 格式，直接视为校验失败
            return false;
        }
    }
}
