namespace DailyCost.Domain.Entities;

public class Family
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? InviteCode { get; set; }

    public Guid CreatedBy { get; set; }
    public User Creator { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<FamilyMember> Members { get; set; } = new List<FamilyMember>();
}

