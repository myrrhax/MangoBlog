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
}
