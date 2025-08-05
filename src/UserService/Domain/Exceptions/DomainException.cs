using System;

namespace UserService.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class UserDomainException : DomainException
{
    public UserDomainException(string message) : base(message) { }
    
    public UserDomainException(string message, Exception innerException) : base(message, innerException) { }
}
