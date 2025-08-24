using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PaymentGateway.Api.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var request = context.HttpContext.Request;
        var url = $"{request.Method} {request.Path}{request.QueryString}";
        var body = string.Empty;
        if (request.ContentLength > 0 && request.Body.CanSeek)
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            body = reader.ReadToEnd();
            request.Body.Position = 0;
        }

        // Log the exception details to trace
        _logger.LogError(context.Exception, "Unhandled exception at {Url}. Input: {Body}", url,
            string.IsNullOrEmpty(body) ? "(no body)" : body);

        var response = new Result
        {
            code = ApiCode.Failure.GetValue(),
            message = "An unexpected error occurred"
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        context.ExceptionHandled = true;
    }
}