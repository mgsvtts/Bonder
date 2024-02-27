namespace Domain.Exceptions;

public sealed class AuthorizationException(string message) : Exception(message);