using Stateless;

namespace Web.Services.Dto;

public sealed class UserState
{
    public BondFilters Filters { get; set; } = new BondFilters();
    public StateMachine<State, Trigger>? StateMachine { get; set; }
}
