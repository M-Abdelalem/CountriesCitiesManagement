using System;
using System.Collections.Generic;
using System.Text;

namespace CountriesCitiesManagement.Application.Exceptions
{
    public sealed class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
