using System;
using System.Collections.Generic;
using System.Text;

namespace Vinyl.DbLayer
{
    public static class ParametersConverterExtensions
    {
        public static string ToParametersDbString(this IDictionary<string, string> map)
        {
            return map == null || map.Count == 0
                ? null
                : Newtonsoft.Json.JsonConvert.SerializeObject(map);
        }

        public static IDictionary<string, string> FromParametersDbString(this string dbString)
        {
            return string.IsNullOrWhiteSpace(dbString)
                ? null
                : Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(dbString);
        }
    }
}
