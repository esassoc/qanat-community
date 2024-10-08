using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Qanat.Common.Util;

public static class StringUtilities
{
    public static string HtmlEncode(this string value)
    {
        return string.IsNullOrWhiteSpace(value) ? value : HttpUtility.HtmlEncode(value);
    }

    public static string HtmlEncodeWithBreaks(this string value)
    {
        var ret = value.HtmlEncode();
        return string.IsNullOrWhiteSpace(ret) ? ret : ret.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "<br/>\r\n");
    }

    public static string SlugifyString(string zoneGroupName)
    {
        Regex rgx = new Regex("[^a-zA-Z0-9 -]");
        var resultingSlug = rgx.Replace(zoneGroupName, "").ToLower().Replace(" ", "-");
        return resultingSlug;
    }

    //MK 8/13/2024 -- I know we want to move away from Newtonsoft eventually but right now I need the trusty JObject.
    public static bool TryParseJObject(this string jsonString, out JObject result)
    {
        try
        {
            result = JObject.Parse(jsonString);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }
}