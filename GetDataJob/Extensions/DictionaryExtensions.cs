using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetDataJob
{
    public static class DictionaryExtensions
    {
        public static V GetSafeValue<T, V>(this IDictionary<T, V> map, T key)
        {
            V res;
            return map.TryGetValue(key, out res) ? res : default(V);
        }
    }
}
