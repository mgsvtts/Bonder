using Ardalis.GuardClauses;

namespace Application.Commands.Operations.Create;

public readonly record struct OperationName
{
    public string Value { get; }
    public OperationName(string value)
    {
        Guard.Against.NullOrEmpty(value);

        Value = value;
    }
    public override string ToString()
    {
        return Value;
    }
}