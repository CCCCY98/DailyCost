using DailyCost.Domain.Enums;

namespace DailyCost.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public string? Nickname { get; set; }
    public string? Avatar { get; set; }

    public CalcMode DefaultCalcMode { get; set; } = CalcMode.Dynamic;
    public string Currency { get; set; } = "CNY";
    public string Timezone { get; set; } = "Asia/Shanghai";

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<ExpenseItem> ExpenseItems { get; set; } = new List<ExpenseItem>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

