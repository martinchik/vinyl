using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vinyl.Common
{
    public class ParseSpecialFields
    {
        private static readonly Regex _regexForNumbers = new Regex(@"^\d+$");
        private static readonly Regex _regexForDecimals = new Regex(@"[^0-9\.]+");
        private static readonly Regex _regexForWord = new Regex("[:!@#$%^&*()}{|\":?><\\;'/.,~]");

        public static string DistinctWords(IEnumerable<string> values, string delimiter = ",")
        {
            if (values?.Any() == true)
            {
                var items = values.Where(_ => !string.IsNullOrEmpty(_)).SelectMany(str => 
                    _regexForWord.Replace(str, string.Empty)
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(_ => _.Length > 2 ? _.Trim().ToLower() : string.Empty)
                    .Where(_ => !string.IsNullOrEmpty(_))
                ).Distinct().ToArray();

                return items.Length == 1 ? items[0] : string.Join(delimiter, items);
            }
            return string.Empty;
        }

        public static string DistinctWords(string value, string delimiter = ",")
            =>  string.IsNullOrWhiteSpace(value) 
                ? string.Empty
                : DistinctWords(new[] { value }, delimiter);        

        public static string ParseRecordName(string @value)
        {
            if (!string.IsNullOrEmpty(@value))
            {
                @value = @value.Replace("LP 2", string.Empty).Replace("2 LP", string.Empty);
                @value = RemovePartFromString(@value, '(', ')', 3);
                @value = RemovePartFromString(@value, '[', ']', 1);
                @value = RemovePartFromString(@value, '=');

                var items = @value
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(_ =>
                    {
                        var it = _.ToLower().Trim();
                        if (it.Length == 0)
                            return string.Empty;
                        if (it.Contains("lp") && it.Length < 7)
                            return string.Empty;
                        if (it.Contains("(") && it.Contains(")"))
                            return string.Empty;

                        return _.Trim();
                    })
                    .Where(_ => _ != null && _.Length > 0 && !(_.Length == 1 && (int)_[0] == 8206))
                    .ToArray();

                return (items.Length == 1 ? items[0] : string.Join(" ", items)).Trim();
            }
            return string.Empty;
        }

        public static string RemovePartFromString(string @value, char started, char? ended, int countTimes = 1)
        {
            var str = @value;
            for(int i=0; i < countTimes; i++)
                str = RemovePartFromString(str, started, ended);
            return str;
        }

        public static string RemovePartFromString(string @value, char started, char? ended = null)
        {
            if (!string.IsNullOrEmpty(@value))
            {
                var sind = @value.IndexOf(started);
                if (sind >= 0)
                {
                    var lPart = value.Substring(0, sind).Trim();
                    var send = ended == null ? -1 : @value.IndexOf(ended.Value, sind + 1);
                       
                    if (send > 0)
                    {
                        var rPart = (send + 2) < value.Length ? value.Substring(send + 1, value.Length - send - 1).Trim() : string.Empty;
                        if (!string.IsNullOrEmpty(rPart))
                            return lPart.Trim() + " " + rPart.Trim();
                    }
                    return lPart.Trim();
                }
            }
            return @value;
        }

        public static int? ParseYear(string @value)
        {
            if (!string.IsNullOrEmpty(@value))
            {
                var nambersOnly = _regexForNumbers.Match(@value).Value;
                if (!string.IsNullOrEmpty(nambersOnly))
                {
                    int year;
                    if (int.TryParse(nambersOnly, out year) && year > 1000 && year < 9000)
                        return year;
                }
            }
            return (int?)null;
        }

        public static (decimal? price, string currency) ParsePrice(string @value, string defaultCurrency = "")
        {
            decimal? priceValue = null;
            string currencyValue = null;
            if (!string.IsNullOrEmpty(@value))
            {
                var priceStr = @value.Replace(",", ".");
                var nambersOnly = _regexForDecimals.Replace(priceStr, string.Empty);
                if (!string.IsNullOrEmpty(nambersOnly))
                {
                    decimal price;
                    if (decimal.TryParse(nambersOnly, out price) && price > 0 && price < 100_000)
                        priceValue = price;
                }

                currencyValue = _regexForDecimals.Match(priceStr).Value.Replace(".", string.Empty).Replace(",", string.Empty).Trim();
                if (string.IsNullOrEmpty(currencyValue))
                    currencyValue = defaultCurrency;
                else
                    currencyValue = ValidateCurrency(currencyValue);
            }

            return (price: priceValue, currency: currencyValue);
        }

        public static string ParseState(string @value)
        {
            var state = @value?.Trim().ToUpper() ?? string.Empty;
            if (state.Length > 0)
            {
                var currentStates = state
                    .Split("/")
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(_ => _.Trim().ToUpper())
                    .Distinct()
                    .ToArray();

                state = currentStates.Length > 1 
                        ? string.Join("/", state)
                        : currentStates.First();
            }
            return state;
        }

        private static string ValidateCurrency(string currencyValue)
        {
            if (string.IsNullOrEmpty(currencyValue))
                return string.Empty;

            switch (currencyValue.Replace(".", string.Empty).Replace(",", string.Empty).Replace(" ", string.Empty).ToLower())
            {
                case "р":
                case "роср":
                case "рос":
                case "rus":
                case "rur":
                case "r":
                    return "RUB";
                case "б":
                case "рб":
                case "бр":
                case "белруб":
                case "белр":
                case "бел":
                case "бруб":
                case "b":
                case "br":
                case "byr":
                case "bel":
                    return "BYN";
                case "дол":
                case "$":
                case "услед":
                case "усл":
                    return "USD";
                case "евро":
                case "евр":
                case "euro":
                case "er":
                    return "EUR";
                default:
                    return string.Empty;
            }
        }
    }
}
