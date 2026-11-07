namespace AlMaystro.API.Models;

public enum BookingStatus { Pending = 0, Confirmed = 1, Cancelled = 2, Completed = 3, NoShow = 4 }
public enum BookingSource { Web = 0, Desktop = 1, Mobile = 2 }

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int SortOrder { get; set; }
    public List<Service> Services { get; set; } = new();
}

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    // Fixed price => PriceFrom only (PriceTo null). Range => both.
    public decimal PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
