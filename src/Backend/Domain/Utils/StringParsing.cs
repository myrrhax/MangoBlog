using System.Reflection.Metadata.Ecma335;
using Domain.Enums;

namespace Domain.Utils;

public static class StringParsing
{
    public static SortType ParseSortType(string type)
        => type.Trim().ToLower() switch
        {
            "asc" => SortType.Ascending,
            "desc" => SortType.Descending,
            _ => SortType.None,
        };

    public static bool TryParseRatingType(string type, out RatingType? rating)
    {
        switch(type)
        {
            case "like":
                rating = RatingType.Like;
                break;
            case "dislike":
                rating = RatingType.Dislike;
                break;
            default:
                rating = null;
                return false;
        }

        return true;
    }

    public static SubscriptionType ParseSubscription(string subscription)
        => subscription switch
        {
            "sub" => SubscriptionType.Subscribe,
            "unsub" => SubscriptionType.Unsubscribe,
            _ => throw new ArgumentException("Invalid value for subscription")
        };
}
