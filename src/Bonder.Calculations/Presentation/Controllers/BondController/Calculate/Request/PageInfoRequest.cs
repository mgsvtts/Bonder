namespace Presentation.Controllers.BondController.Calculate.Request;

public sealed record PageInfoRequest(int CurrentPage = 1, int ItemsOnPage = 20);