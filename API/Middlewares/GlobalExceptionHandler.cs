using System.Net;
using API.DTOs;
using API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            this._logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var response = APIResponse<object>.Builder()
                .WithSuccess(false)
                .Build();
            if (exception is AppException e)
            {
                // If the exception is a BaseException, we can extract the error details
                Error error = e.Error;
                httpContext.Response.StatusCode = (int)error.StatusCode;
                response.StatusCode = error.StatusCode;
                response.ErrorMessages = new List<string> { e.Message };
            }
            else
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages = new List<string> { "An unexpected error occurred. Please try again later." };
            }
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken).ConfigureAwait(false);
            return true;

        }
    }
}
