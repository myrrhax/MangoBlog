namespace Domain.Utils.Errors;

public class EmailOrLoginAlreadyTaken(string login, string email) 
    : Error($"Email: {email} or login: {login} already taken");
