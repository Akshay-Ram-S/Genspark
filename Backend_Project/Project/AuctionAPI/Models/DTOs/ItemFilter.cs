public class ItemFilter
{
    public string? Category { get; set; }
    public decimal? StartingPrice { get; set; }
    public decimal? EndingPrice { get; set; }
    public string? Search { get; set; }
    public DateOnly? EndDateBefore { get; set; }
}
