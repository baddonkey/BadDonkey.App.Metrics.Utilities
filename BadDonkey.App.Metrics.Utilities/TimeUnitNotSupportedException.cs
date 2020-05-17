using System;
using App.Metrics;

namespace BadDonkey.App.Metrics.Utilities
{
    public class TimeUnitNotSupportedException : Exception
    {
        public TimeUnitNotSupportedException(TimeUnit timeUnit, Exception innerException = null) : base($"Time unit {timeUnit} not supported.", innerException)
        {
            CurrentTimeUnit = timeUnit;
        }

        public TimeUnit CurrentTimeUnit { get; }
    }
}
