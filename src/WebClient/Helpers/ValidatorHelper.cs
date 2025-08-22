using System.ComponentModel.DataAnnotations;

namespace WebClient.Helpers;

public static class ValidatorHelper
{
    public static string ValidateObject(this object model)
    {
        var errorResult = "";
        List<ValidationResult> errors = new List<ValidationResult>();
        ValidationContext context = new ValidationContext(model, null, null);

        if (!Validator.TryValidateObject(model, context, errors, true))
        {
            foreach (ValidationResult e in errors)
            {
                errorResult = e.ErrorMessage;
                return errorResult;
            }
            return errorResult;
        }
        return errorResult;
    }
}