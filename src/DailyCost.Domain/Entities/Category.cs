namespace DailyCost.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public string Name { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Color { get; set; }

    public bool IsSystem { get; set; }
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<ExpenseItem> ExpenseItems { get; set; } = new List<ExpenseItem>();
}

