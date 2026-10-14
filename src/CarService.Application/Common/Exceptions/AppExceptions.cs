namespace CarService.Application.Common.Exceptions;

public class ValidationException(IDictionary<string, string[]> errors)
    : Exception("Bir yoki bir nechta maydon noto'g'ri to'ldirilgan.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}

public class NotFoundException(string message) : Exception(message);

public class UnauthorizedException(string message) : Exception(message);

public class ConflictException(string message) : Exception(message);
