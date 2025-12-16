using DailyCost.Application.Common;
using DailyCost.Application.DTOs.User;

namespace DailyCost.Application.Services;

public interface IUserService
{
    Task<Result<UserDto>> GetMeAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<UserDto>> UpdateMeAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken);
    Task<Result<object>> ChangePasswordAsync(Guid userId, UpdatePasswordRequest request, CancellationToken cancellationToken);
    Task<Result<UserDto>> UpdateSettingsAsync(Guid userId, UpdateSettingsRequest request, CancellationToken cancellationToken);
}

