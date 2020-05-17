using System;
using System.Diagnostics;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Timer;

namespace BadDonkey.App.Metrics.Utilities
{
    public class TimeRecorder : IAsyncDisposable
    {
        private readonly IMetricsRoot _metricsRoot;
        private readonly TimerOptions _timerOptions;
        private readonly MetricTags _tags;
        private readonly Stopwatch _stopwatch;
        
        private readonly string _userValue;
        private readonly bool _autoFlush;
        private readonly TimeUnit _timeUnit;

        public TimeRecorder(IMetricsRoot metricsRoot, TimerOptions timerOptions, MetricTags tags, TimeUnit timeUnit, bool autoFlush = false, string userValue = null )
        {
            _stopwatch = Stopwatch.StartNew();
            _metricsRoot = metricsRoot;
            _timerOptions = timerOptions;
            _tags = tags;
            _autoFlush = autoFlush;
            _userValue = userValue;
            _timeUnit = timeUnit;
        }

        public ITimer GetTimerInstance(IMetrics metrics, TimerOptions timer, MetricTags tags)
        {
            return metrics.Provider.Timer.Instance(timer, tags);
        }

        public async ValueTask DisposeAsync()
        {
            var timer = GetTimerInstance(_metricsRoot, _timerOptions, _tags);

            _stopwatch.Stop();
            timer.Record(ConvertElapsedTimeToTimeUnit(), TimeUnit.Milliseconds, _userValue);

            if (_autoFlush)
                await Task.WhenAll(_metricsRoot.ReportRunner.RunAllAsync());
        }

        private long ConvertElapsedTimeToTimeUnit()
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (_timeUnit)
            {
                case TimeUnit.Milliseconds:
                    return (long)_stopwatch.Elapsed.TotalMilliseconds;
                case TimeUnit.Seconds:
                    return _stopwatch.Elapsed.Seconds;
                case TimeUnit.Minutes:
                    return _stopwatch.Elapsed.Minutes;
                case TimeUnit.Hours:
                    return _stopwatch.Elapsed.Hours;
                case TimeUnit.Days:
                    return _stopwatch.Elapsed.Days;
                default:
                    throw new TimeUnitNotSupportedException(_timeUnit);
            }
        }
    }
}
