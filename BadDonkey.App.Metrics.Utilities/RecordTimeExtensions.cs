using App.Metrics;
using App.Metrics.Timer;

namespace BadDonkey.App.Metrics.Utilities
{
    public static class RecordTimeExtensions
    {
        public static TimeRecorder RecordTime(this IMetricsRoot metricsRoot, TimerOptions timerOptions, MetricTags metricTags, TimeUnit timeUnit, bool reportAutoFlush = false, string userValue = null)
        {
            return new TimeRecorder(metricsRoot, timerOptions, metricTags, timeUnit, userValue: userValue, autoFlush: reportAutoFlush);
        }
    }
}
