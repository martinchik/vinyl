﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Vinyl.RecordProcessingJob.Data
{
    public class ParseSpecialFields
    {
        private static readonly Regex _regexForNumbers = new Regex(@"^\d$");
        private static readonly Regex _regexForDecimals = new Regex(@"[^0-9\.]+");

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