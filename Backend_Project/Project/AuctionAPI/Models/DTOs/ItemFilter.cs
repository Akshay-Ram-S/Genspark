public class ItemFilter
{
    public string? Category { get; set; }
    public decimal? PriceLessThan { get; set; }
    public string? Search { get; set; }
    public DateOnly? EndDateBefore { get; set; }
}
