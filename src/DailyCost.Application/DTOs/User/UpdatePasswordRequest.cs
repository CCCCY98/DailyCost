namespace DailyCost.Application.DTOs.User;

public sealed class UpdatePasswordRequest
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

