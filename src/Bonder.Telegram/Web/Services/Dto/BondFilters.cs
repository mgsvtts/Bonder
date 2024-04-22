namespace Web.Services.Dto;

public sealed class BondFilters
{
    public DateTime? StartDate { get; set; }
    public decimal PriceFrom { get; set; } = 0;
    public decimal PriceTo { get; set; } = decimal.MaxValue;
    public decimal RatingFrom { get; set; } = 0;
    public decimal RatingTo { get; set; } = 10;
    public DateOnly DateFrom { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly DateTo { get; set; } = DateOnly.MaxValue;
    public bool IncludeUnknownRatings { get; set; } = true;
    public Trigger NextTrigger { get; set; } = Trigger.GetPriceFrom;

    public bool CanProcess => StartDate != null && StartDate.Value.AddMinutes(1) > DateTime.Now;

    public void UpdateNextTrigger(Trigger trigger)
    {
        NextTrigger = trigger switch
        {
            Trigger.Start => Trigger.GetPriceFrom,
            Trigger.GetPriceFrom => Trigger.GetPriceTo,
            Trigger.GetPriceTo => Trigger.GetRatingFrom,
            Trigger.GetRatingFrom => Trigger.GetRatingTo,
            Trigger.GetRatingTo => Trigger.GetDateFrom,
            Trigger.GetDateFrom => Trigger.GetDateTo,
            Trigger.GetDateTo => Trigger.Finish,
            Trigger.Finish => Trigger.Start,
            _ => throw new NotImplementedException(),
        }; 
    }
}
