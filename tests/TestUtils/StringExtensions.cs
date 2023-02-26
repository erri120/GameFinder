using System.Text;

namespace TestUtils;

public static class StringExtensions
{
    public static string ToEscapedString(this string s)
    {
        var sb = new StringBuilder(s);

        sb.Replace("\\", "\\\\");

        return sb.ToString();
    }
}
