namespace TelegramBot.Routing.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class CommandAttribute : Attribute
{
    public string Command { get; }

    public CommandAttribute(string command)
    {
        Command = command.ToLower();
    }
}
