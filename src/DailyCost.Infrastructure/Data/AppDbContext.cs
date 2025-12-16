using DailyCost.Application.Abstractions;
using DailyCost.Domain.Entities;
using DailyCost.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DailyCost.Infrastructure.Data;

public sealed class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ExpenseItem> ExpenseItems => Set<ExpenseItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Family> Families => Set<Family>();
    public DbSet<FamilyMember> FamilyMembers => Set<FamilyMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsers(modelBuilder);
        ConfigureCategories(modelBuilder);
        ConfigureExpenseItems(modelBuilder);
        ConfigureFamilies(modelBuilder);
        ConfigureFamilyMembers(modelBuilder);
        ConfigureRefreshTokens(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<User>();
        e.ToTable("Users");
        e.HasKey(x => x.Id);
        e.Property(x => x.Id).HasColumnType("char(36)").IsRequired();
        e.Property(x => x.Email).HasMaxLength(255).IsRequired();
        e.HasIndex(x => x.Email).IsUnique();
        e.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
        e.Property(x => x.Nickname).HasMaxLength(50);
        e.Property(x => x.Avatar).HasMaxLength(500);
        e.Property(x => x.DefaultCalcMode).HasConversion<byte>().HasColumnType("tinyint");
        e.Property(x => x.Currency).HasMaxLength(10).HasDefaultValue("CNY");
        e.Property(x => x.Timezone).HasMaxLength(50).HasDefaultValue("Asia/Shanghai");
        e.Property(x => x.CreatedAt).IsRequired();
        e.Property(x => x.UpdatedAt).IsRequired();
        e.Property(x => x.LastLoginAt);
        e.Property(x => x.IsDeleted).HasDefaultValue(false);
    }

    private static void ConfigureCategories(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<Category>();
        e.ToTable("Categories");
        e.HasKey(x => x.Id);
        e.Property(x => x.Id).HasColumnType("char(36)").IsRequired();
        e.Property(x => x.UserId).HasColumnType("char(36)");
        e.Property(x => x.Name).HasMaxLength(50).IsRequired();
        e.Property(x => x.Icon).HasMaxLength(100);
        e.Property(x => x.Color).HasMaxLength(20);
        e.Property(x => x.IsSystem).HasDefaultValue(false);
        e.Property(x => x.SortOrder).HasDefaultValue(0);
        e.Property(x => x.CreatedAt).IsRequired();
        e.Property(x => x.UpdatedAt).IsRequired();
        e.Property(x => x.IsDeleted).HasDefaultValue(false);
        e.HasOne(x => x.User).WithMany(u => u.Categories).HasForeignKey(x => x.UserId);
    }

    private static void ConfigureExpenseItems(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<ExpenseItem>();
        e.ToTable("ExpenseItems");
        e.HasKey(x => x.Id);
        e.Property(x => x.Id).HasColumnType("char(36)").IsRequired();
        e.Property(x => x.UserId).HasColumnType("char(36)").IsRequired();
        e.Property(x => x.CategoryId).HasColumnType("char(36)");
        e.Property(x => x.FamilyId).HasColumnType("char(36)");

        e.Property(x => x.Name).HasMaxLength(100).IsRequired();
        e.Property(x => x.Amount).HasColumnType("decimal(12,2)").IsRequired();
        e.Property(x => x.ExpenseType).HasConversion<byte>().HasColumnType("tinyint").IsRequired();

        e.Property(x => x.StartDate).HasColumnType("date").IsRequired();
        e.Property(x => x.EndDate).HasColumnType("date");
        e.Property(x => x.ExpectedDays);

        e.Property(x => x.BillingCycle).HasConversion<byte?>().HasColumnType("tinyint");
        e.Property(x => x.AutoRenew).HasDefaultValue(true);
        e.Property(x => x.NextRenewalDate).HasColumnType("date");

        e.Property(x => x.CalcMode).HasConversion<byte?>().HasColumnType("tinyint");
        e.Property(x => x.Status).HasConversion<byte>().HasColumnType("tinyint");

        e.Property(x => x.Note).HasColumnType("text");
        e.Property(x => x.ImageUrl).HasMaxLength(500);
        e.Property(x => x.Tags).HasMaxLength(500);

        e.Property(x => x.CreatedAt).IsRequired();
        e.Property(x => x.UpdatedAt).IsRequired();
        e.Property(x => x.DeletedAt);

        e.HasOne(x => x.User).WithMany(u => u.ExpenseItems).HasForeignKey(x => x.UserId);
        e.HasOne(x => x.Category).WithMany(c => c.ExpenseItems).HasForeignKey(x => x.CategoryId);
        e.HasIndex(x => new { x.UserId, x.Status }).HasDatabaseName("idx_user_status");
        e.HasIndex(x => new { x.UserId, x.CategoryId }).HasDatabaseName("idx_user_category");
    }

    private static void ConfigureFamilies(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<Family>();
        e.ToTable("Families");
        e.HasKey(x => x.Id);
        e.Property(x => x.Id).HasColumnType("char(36)").IsRequired();
        e.Property(x => x.Name).HasMaxLength(50).IsRequired();
        e.Property(x => x.InviteCode).HasMaxLength(20);
        e.HasIndex(x => x.InviteCode).IsUnique();
        e.Property(x => x.CreatedBy).HasColumnType("char(36)").IsRequired();
        e.Property(x => x.CreatedAt).IsRequired();
        e.Property(x => x.UpdatedAt).IsRequired();
        e.Property(x => x.IsDeleted).HasDefaultValue(false);
        e.HasOne(x => x.Creator).WithMany().HasForeignKey(x => x.CreatedBy);
    }

    private static void ConfigureFamilyMembers(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<FamilyMember>();
        e.ToTable("FamilyMembers");
        e.HasKey(x => x.Id);
        e.Property(x => x.Id).HasColumnType("char(36)").IsRequired();
        e.Property(x => x.FamilyId).HasColumnType("char(36)").IsRequired();
        e.Property(x => x.UserId).HasColumnType("char(36)").IsRequired();
        e.Property(x => x.Role).HasColumnType("tinyint").HasDefaultValue((byte)0);
        e.Property(x => x.JoinedAt).IsRequired();
        e.HasIndex(x => new { x.FamilyId, x.UserId }).IsUnique().HasDatabaseName("uk_family_user");
        e.HasOne(x => x.Family).WithMany(f => f.Members).HasForeignKey(x => x.FamilyId);
        e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }

    private static void ConfigureRefreshTokens(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<RefreshToken>();
        e.ToTable("RefreshTokens");
        e.HasKey(x => x.Id);
        e.Property(x => x.Id).HasColumnType("char(36)").IsRequired();
        e.Property(x => x.UserId).HasColumnType("char(36)").IsRequired();
        e.Property(x => x.Token).HasMaxLength(500).IsRequired();
        e.Property(x => x.ExpiresAt).IsRequired();
        e.Property(x => x.CreatedAt).IsRequired();
        e.Property(x => x.RevokedAt);
        e.HasOne(x => x.User).WithMany(u => u.RefreshTokens).HasForeignKey(x => x.UserId);
        e.HasIndex(x => x.Token).HasDatabaseName("idx_token");
    }
}
