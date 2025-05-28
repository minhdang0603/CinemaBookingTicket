using System.Net;
using CinemaBookingTicket_API.DTO;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CinemaBookingTicket_API.Middlewares
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
            if (exception is BaseException e)
            {
                httpContext.Response.StatusCode = (int)e.StatusCode;
                response.StatusCode = e.StatusCode;
                response.ErrorMessages = e.Message;
            }
            else
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages = "Internal Server Error. Please try again later.";
            }
            _logger.LogError(exception, exception.Message);
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken).ConfigureAwait(false);
            return true;

        }
    }
}
