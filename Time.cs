using System;

namespace EB_Utility
{
    public static class Time
    {
        private static TimeSpan GetEpochTime()
        {
            return DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
        }

        public static long GetTimestampSec()
        {
            return (long)GetEpochTime().TotalSeconds;
        }

        public static long GetTimestampMS()
        {
            return (long)GetEpochTime().TotalMilliseconds;
        }
    }
}
