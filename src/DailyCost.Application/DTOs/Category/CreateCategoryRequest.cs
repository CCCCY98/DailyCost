namespace DailyCost.Application.DTOs.Category;

public sealed class CreateCategoryRequest
{
    public string Name { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int SortOrder { get; set; }
}

