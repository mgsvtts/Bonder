namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct Dates
{
    public DateTime? MaturityDate { get; }
    public DateTime? OfferDate { get; }

    public Dates(DateTime? maturityDate, DateTime? offerDate = null)
    {
        MaturityDate = maturityDate;
        OfferDate = offerDate;
    }
}