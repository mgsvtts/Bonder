namespace Web.Services.Dto;

public sealed record BondFilters
{
    public static BondFilters Default => new();

    public DateTime? StartDate { get; set; }
    public decimal PriceFrom { get; set; } = 0;
    public decimal PriceTo { get; set; } = int.MaxValue;
    public int RatingFrom { get; set; } = 0;
    public int RatingTo { get; set; } = 10;
    public DateOnly? DateFrom { get; set; } = null;
    public DateOnly DateTo { get; set; } = DateOnly.MaxValue;
    public DateToType DateToType { get; set; } = DateToType.Custom;
    public bool IncludeUnknownRatings { get; set; } = true;
}