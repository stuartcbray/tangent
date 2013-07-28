using System;
using MonoTouch.Foundation;

namespace TheFactorM.Federation
{
	public static class DateTimeExtensions
	{
		public static DateTime ToDateTime(this MonoTouch.Foundation.NSDate date)
		{
		    return (new DateTime(2001,1,1,0,0,0)).AddSeconds(date.SecondsSinceReferenceDate);
		}
		
		public static MonoTouch.Foundation.NSDate ToNSDate(this DateTime date)
		{
		    return MonoTouch.Foundation.NSDate.FromTimeIntervalSinceReferenceDate((date-(new DateTime(2001,1,1,0,0,0))).TotalSeconds);
		}
	}
}

