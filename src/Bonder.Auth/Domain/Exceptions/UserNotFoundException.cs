namespace Domain.Exceptions;

public sealed class UserNotFoundException(string userName) : Exception($"User {userName} does not exist")
{ }