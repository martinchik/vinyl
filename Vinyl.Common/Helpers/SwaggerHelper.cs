using System;
using System.Collections.Generic;
using System.Text;

namespace Vinyl.Common.Helpers
{
    public static class SwaggerHelper
    {
        public static string GetSwaggerPrefix()
        {
            var swaggerPrefix = Environment.GetEnvironmentVariable("PROXY_PREFIX");
            return !string.IsNullOrEmpty(swaggerPrefix)
                ? swaggerPrefix + "/"
                : string.Empty;
        }

        public static string ApiChangeRelativePath(string apiRelativePath)
        {
            var swaggerPrefix = GetSwaggerPrefix();
            if (!apiRelativePath.Trim().Contains(swaggerPrefix))
                return swaggerPrefix + apiRelativePath;

            return apiRelativePath;
        }
    }
}
