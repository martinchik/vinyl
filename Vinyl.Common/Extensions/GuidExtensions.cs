using System;
using System.Collections.Generic;
using System.Text;

namespace Vinyl
{
    public static class GuidExtensions
    {
        public static string Compress(this Guid guid)
        {
            // "MspzuC2oLkKjoUEgZrbJFg=="
            return Convert.ToBase64String(guid.ToByteArray())
                .Substring(0, 22)
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static Guid ExpandToGuid(this string compressedGuid)
        {
            string base64 = compressedGuid
                .Replace('_', '/')
                .Replace('-', '+')
                + "==";

            var bytes = Convert.FromBase64String(base64);
            return new Guid(bytes);
        }
    }
}
