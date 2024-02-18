using Ardalis.GuardClauses;
using Domain.BondAggreagte.Dto.Guards;

namespace Domain.BondAggreagte.Abstractions.Dto;

public readonly record struct PageInfo
{
    public const int MaxItemsOnPage = 50;

    public int CurrentPage { get; }
    public int LastPage { get; }
    public int ItemsOnPage { get; }
    public int Total { get; }

    public PageInfo(int currentPage,
                    int lastPage,
                    int itemsOnPage,
                    int total) : this(currentPage, itemsOnPage)
    {
        LastPage = Guard.Against.NegativeOrZero(lastPage);
        Total = Guard.Against.Negative(total);
    }

    public PageInfo(int currentPage, int itemsOnPage)
    {
        CurrentPage = Guard.Against.NegativeOrZero(currentPage);
        ItemsOnPage = Guard.Against.MoreThanMaxItemsOnPage(itemsOnPage, MaxItemsOnPage);
    }
}