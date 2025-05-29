namespace Domain.Utils.Errors;

public class ApplicationValidationError : Error
{
    private Dictionary<string, string> _errors;
    public IReadOnlyDictionary<string, string> Errors => _errors;

    public ApplicationValidationError(Dictionary<string, string> errors)
        : base("One or more validation errors occurred during execution") 
    {
        _errors = errors;
    }
}
