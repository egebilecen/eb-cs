public static class StringExtension
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
}
