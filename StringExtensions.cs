using System.Linq;

public static class StringExtensions
{
    public static string KeyFormat(this string str, params (string, object)[] formatPair)
    {
        if(formatPair.Length < 1) return str;

        foreach((string, object) pair in formatPair)
        {
            str = str.Replace("{" + pair.Item1 + "}", pair.Item2.ToString());
        }

        return str;
    }

    public static string FirstCharacterToUpper(this string str)
    {
        if(string.IsNullOrEmpty(str)) return str;
        return str.First().ToString().ToUpper() + str.Substring(1);
    }

    public static string WordFirstCharacterToUpper(this string str)
    {
        if(string.IsNullOrEmpty(str)) return str;
        return string.Join(" ", str.Split(' ').Select(x => x.FirstCharacterToUpper()));
    }
}
