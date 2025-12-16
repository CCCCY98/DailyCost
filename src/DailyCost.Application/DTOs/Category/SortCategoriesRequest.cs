namespace DailyCost.Application.DTOs.Category;

public sealed class SortCategoriesRequest
{
    public List<SortItem> Items { get; set; } = new();

    public sealed class SortItem
    {
        public Guid Id { get; set; }
        public int SortOrder { get; set; }
    }
}

