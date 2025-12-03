namespace Common.Application.Requests.Sorting;

public record SortRequest<T>(T Value) where T : struct, Enum
{
    public static bool TryParse(string value, IFormatProvider provider, out SortRequest<T> val)
    {
        if (Enum.TryParse<T>(value, ignoreCase: true, out var enumVal))
        {
            val = new SortRequest<T>(enumVal);
            return true;
        }

        val = null;
        return false;
    }
}