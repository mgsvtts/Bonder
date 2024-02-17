namespace Domain.Exceptions;

public class UserNotFoundException(string userName) : Exception($"User {userName} does not exist")
{ }