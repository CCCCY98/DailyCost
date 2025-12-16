using AutoMapper;
using DailyCost.Application.Abstractions;
using DailyCost.Application.Common;
using DailyCost.Application.DTOs.User;
using DailyCost.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DailyCost.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public UserService(IAppDbContext db, IDateTimeProvider dateTimeProvider, IMapper mapper)
    {
        _db = db;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> GetMeAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<UserDto>.Fail("用户不存在");
        return Result<UserDto>.Ok(_mapper.Map<UserDto>(user));
    }

    public async Task<Result<UserDto>> UpdateMeAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<UserDto>.Fail("用户不存在");

        user.Nickname = request.Nickname;
        user.Avatar = request.Avatar;
        user.UpdatedAt = _dateTimeProvider.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        return Result<UserDto>.Ok(_mapper.Map<UserDto>(user));
    }

    public async Task<Result<object>> ChangePasswordAsync(Guid userId, UpdatePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<object>.Fail("用户不存在");

        try
        {
            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
            if (verify == PasswordVerificationResult.Failed) return Result<object>.Fail("当前密码错误");
        }
        catch (FormatException)
        {
            return Result<object>.Fail("当前密码错误");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        user.UpdatedAt = _dateTimeProvider.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result<object>.Ok(new { });
    }

    public async Task<Result<UserDto>> UpdateSettingsAsync(Guid userId, UpdateSettingsRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<UserDto>.Fail("用户不存在");

        user.DefaultCalcMode = request.DefaultCalcMode;
        user.Currency = request.Currency;
        user.Timezone = request.Timezone;
        user.UpdatedAt = _dateTimeProvider.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        return Result<UserDto>.Ok(_mapper.Map<UserDto>(user));
    }
}
