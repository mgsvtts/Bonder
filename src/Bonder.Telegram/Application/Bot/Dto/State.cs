namespace Application.Bot.Dto;

public enum State
{
    Starting,
    GettingPriceFrom,
    GettingPriceTo,
    GettingRatingFrom,
    GettingRatingTo,
    GettingUnknownRatings,
    GettingDateFrom,
    GettingDateTo,
    Finished
}