using Microsoft.AspNetCore.Mvc.Filters;

namespace PaymentGateway.Api.Filters;

public class ValidateModelFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            context.Result = new BadRequestObjectResult(new Result
            {
                code = ApiCode.Failure.GetValue(),
                message = "Validation failed",
                data = errors
            });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}