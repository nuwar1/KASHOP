using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Diagnostics;

namespace KASHOP.PL
{
    public class GlobalExeptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var result = new Result<object>
            {
                Success = true,
                Message = exception.InnerException.Message
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);

            return true; 
        }
    }
}
