﻿using System.Net;
using API.Exceptions;

namespace API.Exceptions
{
    public class AppException : Exception
    {
        public Error Error { get; }

        public AppException(Error error) : base(error.Message)
        {
            Error = error;
        }
    }
}
