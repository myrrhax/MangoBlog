namespace Domain.Utils.Errors;

public class LoginAlreadyTakenError(string login) 
    : Error($"Login: {login} is already taken");
