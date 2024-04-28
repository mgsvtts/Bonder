using Stateless;

namespace Application.Bot.Dto;

public sealed class UserState
{
    public BondFilters Filters { get; set; } = BondFilters.Default;
    public StateMachine<State, Trigger>? StateMachine { get; set; }
}