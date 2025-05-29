namespace Domain.Utils.Errors;

public class EmailAlreadyExists(string email) 
    : Error($"Email: {email} is already taken");
