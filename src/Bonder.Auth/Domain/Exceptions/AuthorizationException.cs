namespace Domain.Exceptions;

public class AuthorizationException(string message) : Exception(message);