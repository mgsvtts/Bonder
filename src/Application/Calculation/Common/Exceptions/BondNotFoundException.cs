namespace Application.Calculation.Common.Exceptions;

public class BondNotFoundException(string id)
    : Exception($"Bond with ID: {id} not found")
{ }