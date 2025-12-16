using DailyCost.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyCost.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedSystemCategoriesAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        var hasSystem = await db.Categories.AnyAsync(c => c.IsSystem && !c.IsDeleted, cancellationToken);
        if (hasSystem) return;

        var now = DateTime.UtcNow;
        var seed = new List<Category>
        {
            New("电子产品", "device", "#4F46E5", 10),
            New("订阅服务", "subscription", "#0EA5E9", 20),
            New("生活家居", "home", "#10B981", 30),
            New("出行交通", "transport", "#F59E0B", 40),
            New("服饰穿搭", "fashion", "#EC4899", 50),
            New("周期账单", "bill", "#8B5CF6", 60),
            New("其他", "other", "#6B7280", 70)
        };

        db.Categories.AddRange(seed);
        await db.SaveChangesAsync(cancellationToken);

        Category New(string name, string icon, string color, int sortOrder) => new()
        {
            Id = Guid.NewGuid(),
            UserId = null,
            Name = name,
            Icon = icon,
            Color = color,
            IsSystem = true,
            SortOrder = sortOrder,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };
    }
}

