using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Exceptions
{
    public class CustomException : Exception
    {
        public int StatusCode { get; }

        protected CustomException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public class NotFoundException : CustomException
        {
            public NotFoundException(string message) : base(message, 404) { }
        }

        public class ValidationException : CustomException
        {
            public ValidationException(string message) : base(message, 400) { }
        }

        public class UnauthorizedException : CustomException
        {
            public UnauthorizedException(string message) : base(message, 401) { }
        }

        public class ForbiddenException : CustomException
        {
            public ForbiddenException(string message) : base(message, 403) { }
        }
    }
}
