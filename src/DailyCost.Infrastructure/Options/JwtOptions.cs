namespace DailyCost.Infrastructure.Options;

public sealed class JwtOptions
{
    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = "DailyCost";
    public string Audience { get; set; } = "DailyCost";

    public int AccessTokenMinutes { get; set; } = 60;
    public int RefreshTokenDays { get; set; } = 30;
    public int PasswordResetMinutes { get; set; } = 15;
}

