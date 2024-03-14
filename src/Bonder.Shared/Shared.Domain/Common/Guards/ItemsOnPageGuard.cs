using Ardalis.GuardClauses;

namespace Shared.Domain.Common.Guards;

public static class ItemsOnPageGuard
{
    public static int MoreThanMaxItemsOnPage(this IGuardClause clause, int itemsOnPage, int maxItemsOnPage)
    {
        if (itemsOnPage < 0)
        {
            throw new ArgumentException($"{nameof(itemsOnPage)} cannot be less than 0", nameof(itemsOnPage));
        }
        if (itemsOnPage > maxItemsOnPage)
        {
            throw new ArgumentException($"{nameof(itemsOnPage)} cannot be more than {maxItemsOnPage}", nameof(itemsOnPage));
        }

        return itemsOnPage;
    }
}