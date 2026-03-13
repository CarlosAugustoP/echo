using EchoProject.Domain.Exception.EchoProject.Domain.Common;

namespace EchoProject.Application.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, string errorCode) : base(message, errorCode)
        {
        }

        public NotFoundException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}