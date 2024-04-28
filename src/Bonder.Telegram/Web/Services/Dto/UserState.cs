using Stateless;

namespace Web.Services.Dto;

public sealed class UserState
{
    public BondFilters Filters { get; set; } = BondFilters.Default;
    public StateMachine<State, Trigger>? StateMachine { get; set; }
}