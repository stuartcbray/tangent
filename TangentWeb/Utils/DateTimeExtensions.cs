using System;

namespace TangentWeb.Utils
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static double GetEpochTicks(this DateTime dateTime)
        {
            return dateTime.Subtract(Epoch).TotalMilliseconds;
        }
    }
}