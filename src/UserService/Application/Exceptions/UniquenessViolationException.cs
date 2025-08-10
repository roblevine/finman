using System;

namespace UserService.Application.Exceptions;

public class UniquenessViolationException : Exception
{
    public UniquenessViolationException(string message) : base(message)
    {
    }

    public UniquenessViolationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
