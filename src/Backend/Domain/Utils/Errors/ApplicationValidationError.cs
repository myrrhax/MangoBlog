namespace Domain.Utils.Errors;

public class ApplicationValidationError : Error
{
    private Dictionary<string, List<string>> _errors;
    public IReadOnlyDictionary<string, List<string>> Errors => _errors;

    public ApplicationValidationError(Dictionary<string, List<string>> errors)
        : base("One or more validation errors occurred during execution") 
    {
        _errors = errors;
    }

    public static ApplicationValidationError ErrorFrom(string field, string message)
    {
        var dict = new Dictionary<string, List<string>>();
        dict[field] = new List<string>() { message };

        return new ApplicationValidationError(dict);
    }
}
