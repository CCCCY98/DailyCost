namespace DailyCost.Domain.Entities;

public class FamilyMember
{
    public Guid Id { get; set; }

    public Guid FamilyId { get; set; }
    public Family Family { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public byte Role { get; set; }
    public DateTime JoinedAt { get; set; }
}

