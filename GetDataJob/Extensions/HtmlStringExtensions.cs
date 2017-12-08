using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GetDataJob
{
    public static class HtmlStringExtensions
    {
        public static string ToNormalValue(this string htmlValue)
        {            
            return HttpUtility.HtmlDecode(htmlValue
                .Replace("\n", string.Empty)
                .Replace("\t", string.Empty)
                .Replace("\r", string.Empty)
                .Trim());
        }

        public static string ToCsvValue(this string val)
        {
            return (val == null ? "" : string.Concat("\"", val, "\"")) + ";";
        }
    }
}
