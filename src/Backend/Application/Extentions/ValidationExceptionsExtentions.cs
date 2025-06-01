using FluentValidation.Results;

namespace Application.Extentions;

internal static class ValidationExceptionsExtentions
{
    public static Dictionary<string, List<string>> ToErrorsDictionary(this IEnumerable<ValidationFailure> failures)
    {
        var result = new Dictionary<string, List<string>>();

        foreach (var failure in failures)
        {
            if (!result.ContainsKey(failure.PropertyName))
            {
                result[failure.PropertyName] = new List<string>();
            }
            result[failure.PropertyName].Add(failure.ErrorMessage);
        }

        return result;
    }
}
