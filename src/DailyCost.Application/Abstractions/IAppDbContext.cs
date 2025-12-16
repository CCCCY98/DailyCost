using DailyCost.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyCost.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Category> Categories { get; }
    DbSet<ExpenseItem> ExpenseItems { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Family> Families { get; }
    DbSet<FamilyMember> FamilyMembers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

