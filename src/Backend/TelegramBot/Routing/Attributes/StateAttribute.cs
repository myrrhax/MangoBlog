using TelegramBot.Context;

namespace TelegramBot.Routing.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class StateAttribute : Attribute
{
    public Type StateType { get; }

    public StateAttribute(Type stateType)
    {
        if (!typeof(State).IsAssignableFrom(stateType))
            throw new ArgumentException($"{stateType.Name} needs to implement IState interface");

        StateType = stateType;
    }
}
