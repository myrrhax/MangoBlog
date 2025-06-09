namespace TelegramBot.Routing.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class StateAttribute : Attribute
{
    public Type Type { get; }
    public Enum Value { get; }

    public StateAttribute(Type type, object value)
    {
        Type = type;
        if (value is not Enum en)
            throw new ArgumentException($"{nameof(value)} must be Enum");
        Value = en;
    }
}
