namespace EchoProject.Domain.Exception
{
   using System;

namespace EchoProject.Domain.Common
{
    public class DomainException : Exception
    {
        public string? ErrorCode { get; }

        public DomainException(string message)
            : base(message)
        {
        }

        public DomainException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
}