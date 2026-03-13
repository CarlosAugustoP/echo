using EchoProject.Domain.Exception.EchoProject.Domain.Common;

namespace EchoProject.Application.Exceptions
{
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException(string message, string errorCode) : base(message, errorCode)
        {
        }

        public UnauthorizedException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

    }
}