using Ardalis.GuardClauses;

namespace Domain.BondAggreagte.Dto.Guards;

public static class ItemsOnPageGuard
{
    public static int MoreThanMaxItemsOnPage(this IGuardClause clause, int itemsOnPage, int maxItemsOnPage)
    {
        if (itemsOnPage <= 0)
        {
            throw new ArgumentException($"{nameof(itemsOnPage)} cannot be less or equal to 0", nameof(itemsOnPage));
        }
        if (itemsOnPage > maxItemsOnPage)
        {
            throw new ArgumentException($"{nameof(itemsOnPage)} cannot be more than {maxItemsOnPage}", nameof(itemsOnPage));
        }

        return itemsOnPage;
    }
}