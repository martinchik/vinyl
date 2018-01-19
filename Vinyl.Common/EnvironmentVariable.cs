using System;
using System.Collections.Generic;
using System.Text;

namespace Vinyl.Common
{
    public static class EnvironmentVariable
    {
        public static string PROXY_PREFIX => Environment.GetEnvironmentVariable("PROXY_PREFIX");
        public static string CONNECTION_STRING => Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        public static string KAFKA_DIRTY_RECORDS_TOPIC => Environment.GetEnvironmentVariable("KAFKA_DIRTY_RECORDS_TOPIC");
        public static string KAFKA_FIND_INFO_RECORDS_TOPIC => Environment.GetEnvironmentVariable("KAFKA_FIND_INFO_RECORDS_TOPIC");
        public static string KAFKA_CONNECT => Environment.GetEnvironmentVariable("KAFKA_CONNECT");
        public static bool CLEAR_DB => Environment.GetEnvironmentVariable("CLEAR_DB") == "1";        
    }
}
