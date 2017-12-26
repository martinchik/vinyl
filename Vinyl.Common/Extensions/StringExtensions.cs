using System;
using System.Collections.Generic;
using System.Text;

namespace Vinyl
{
    public static class StringExtensions
    {
        public static string AddIfExist(this string currentString, string @value)
        {
            if (string.IsNullOrEmpty(value))
                return currentString;

            return string.Concat(currentString, value);
        }

        public static string AddIfExist(this string currentString, string between, string @value, string after = null)
        {
            if (string.IsNullOrEmpty(value))
                return currentString;

            return after == null 
                ? string.Concat(currentString, between, value)
                : string.Concat(currentString, between, value, after);
        }
    }
}