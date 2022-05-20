using System;

namespace EB_Utility
{
    public static class Time
    {
        private static TimeSpan get_epoch_time()
        {
            return DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
        }

        public static long get_timestamp_sec()
        {
            return (long)get_epoch_time().TotalSeconds;
        }

        public static long get_timestamp_ms()
        {
            return (long)get_epoch_time().TotalMilliseconds;
        }
    }
}
