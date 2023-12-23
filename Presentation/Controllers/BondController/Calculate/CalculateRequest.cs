namespace Presentation.Controllers.BondController.Calculate;
public sealed record CalculateRequest(IdType IdType, IEnumerable<string> Ids);

public enum IdType
{
    Ticker,
    UID,
    FIGI
}