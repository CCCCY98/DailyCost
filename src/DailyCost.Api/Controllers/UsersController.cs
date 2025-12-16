using DailyCost.Api.Models;
using DailyCost.Application.DTOs.User;
using DailyCost.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyCost.Api.Controllers;

[Route("api/v1/users")]
[Authorize]
public sealed class UsersController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _env;

    public UsersController(IUserService userService, IWebHostEnvironment env)
    {
        _userService = userService;
        _env = env;
    }

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Me(CancellationToken cancellationToken)
    {
        var result = await _userService.GetMeAsync(GetUserId(), cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<UserDto>.Ok(result.Data))
            : BadRequest(ApiResponse<UserDto>.Fail(result.Message ?? "获取失败"));
    }

    [HttpPut("me")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateMe([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateMeAsync(GetUserId(), request, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<UserDto>.Ok(result.Data))
            : BadRequest(ApiResponse<UserDto>.Fail(result.Message ?? "更新失败"));
    }

    [HttpPut("me/password")]
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] UpdatePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.ChangePasswordAsync(GetUserId(), request, cancellationToken);
        return result.Success ? Ok(ApiResponse.Ok(result.Data)) : BadRequest(ApiResponse.Fail(result.Message ?? "更新失败"));
    }

    [HttpPut("me/settings")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateSettings([FromBody] UpdateSettingsRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateSettingsAsync(GetUserId(), request, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<UserDto>.Ok(result.Data))
            : BadRequest(ApiResponse<UserDto>.Fail(result.Message ?? "更新失败"));
    }

    [HttpPost("me/avatar")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<ApiResponse<UserDto>>> UploadAvatar([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length <= 0) return BadRequest(ApiResponse<UserDto>.Fail("文件为空"));

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var dir = Path.Combine(_env.ContentRootPath, "uploads", "avatars");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, fileName);

        await using (var fs = System.IO.File.OpenWrite(path))
        {
            await file.CopyToAsync(fs, cancellationToken);
        }

        var url = $"/uploads/avatars/{fileName}";
        var result = await _userService.UpdateMeAsync(GetUserId(), new UpdateProfileRequest { Avatar = url }, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<UserDto>.Ok(result.Data))
            : BadRequest(ApiResponse<UserDto>.Fail(result.Message ?? "上传失败"));
    }
}
