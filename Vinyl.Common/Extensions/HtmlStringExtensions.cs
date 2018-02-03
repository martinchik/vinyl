using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Vinyl
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

        public static string WrapHtmlLines(this string htmlValue)
        {
            return htmlValue.Replace("&#13;", Environment.NewLine).Replace("&#10;", Environment.NewLine).Replace("<br", Environment.NewLine)
                .Replace("/>", string.Empty).Replace(">", string.Empty)
                .Replace("\r\n", Environment.NewLine).Replace("\n", Environment.NewLine)
                .Trim();
        }
    }
}
