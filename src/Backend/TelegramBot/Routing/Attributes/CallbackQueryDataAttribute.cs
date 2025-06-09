namespace TelegramBot.Routing.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class CallbackQueryDataAttribute : Attribute
{
    public string Query { get; }

    public CallbackQueryDataAttribute(string query = "")
    {
        Query = query;
    }
}
