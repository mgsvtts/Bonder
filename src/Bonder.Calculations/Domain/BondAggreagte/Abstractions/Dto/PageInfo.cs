using Ardalis.GuardClauses;
using Domain.BondAggreagte.Dto.Guards;

namespace Domain.BondAggreagte.Abstractions.Dto;

public readonly record struct PageInfo
{
    public const int MaxItemsOnPage = 20;

    public int CurrentPage { get; }
    public int LastPage { get; }
    public int ItemsOnPage { get; }
    public int Total { get; }

    public static readonly PageInfo Default = new PageInfo(1);

    public PageInfo(int currentPage,
                    int lastPage,
                    int itemsOnPage,
                    int total) : this(currentPage)
    {
        Total = Guard.Against.Negative(total);
        LastPage = Guard.Against.Negative(lastPage);
        ItemsOnPage = Guard.Against.MoreThanMaxItemsOnPage(itemsOnPage, MaxItemsOnPage);
    }

    public PageInfo(int currentPage)
    {
        ItemsOnPage = MaxItemsOnPage;
        CurrentPage = Guard.Against.NegativeOrZero(currentPage);
    }
}