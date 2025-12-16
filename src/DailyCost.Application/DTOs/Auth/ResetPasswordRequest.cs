namespace DailyCost.Application.DTOs.Auth;

public sealed class ResetPasswordRequest
{
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

