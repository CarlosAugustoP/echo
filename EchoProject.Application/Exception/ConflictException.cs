using EchoProject.Domain.Exception.EchoProject.Domain.Common;

namespace EchoProject.Application.Exception
{
    public class ConflictException : DomainException
    {
        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(string message, string errorCode) : base(message, errorCode)
        {
        }

        public ConflictException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

    }
}