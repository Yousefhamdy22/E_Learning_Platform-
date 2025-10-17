using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper
{
    public class AuthenticationExceptions : Exception
    {
        public string ErrorCode { get; }

        public AuthenticationExceptions(string message, string errorCode = "AUTH_ERROR") : base(message)
        {
            ErrorCode = errorCode;
        }

        public AuthenticationExceptions(string message, Exception innerException, string errorCode = "AUTH_ERROR")
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    public class SingleDeviceViolationException : AuthenticationExceptions
    {
        public SingleDeviceViolationException() : base("This account is already logged in on another device", "SINGLE_DEVICE_VIOLATION")
        {
        }
    }

}
