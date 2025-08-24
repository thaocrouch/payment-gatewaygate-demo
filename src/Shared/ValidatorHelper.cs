using System.ComponentModel.DataAnnotations;

namespace Shared;

public static class ValidatorHelper
{
    public static string ValidateObject(this object model)
    {
        var errorResult = "";
        var errors = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);

        if (!Validator.TryValidateObject(model, context, errors, true))
        {
            foreach (var e in errors)
            {
                errorResult = e.ErrorMessage;
                return errorResult;
            }

            return errorResult;
        }

        return errorResult;
    }
}