using System;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Reporting;
using App.Metrics.Timer;
using FluentAssertions;
using Moq;
using Xunit;

namespace BadDonkey.App.Metrics.Utilities.Tests
{
    public class ShouldRecordTimeWithDifferentUnits
    {
        [Theory]
        [InlineData(TimeUnit.Milliseconds, null)]
        [InlineData(TimeUnit.Seconds, null)]
        [InlineData(TimeUnit.Hours, null)]
        [InlineData(TimeUnit.Days, null)]
        [InlineData(TimeUnit.Microseconds, typeof(TimeUnitNotSupportedException))]
        [InlineData(TimeUnit.Nanoseconds, typeof(TimeUnitNotSupportedException))]
        public async Task ShouldMeasureTime(TimeUnit timeUnit, Type exceptionType)
        {
            var metricsRootMock = new Mock<IMetricsRoot>();
            var metricsRoot = metricsRootMock.Object;

            var runMetricsReportMock = new Mock<IRunMetricsReports>();
            metricsRootMock.Setup(s => s.ReportRunner).Returns(runMetricsReportMock.Object);

            var timerOptions = new TimerOptions();
            var metricTags = new MetricTags();

            var mockTimer = new Mock<ITimer>();
            metricsRootMock.Setup(s => s.Provider.Timer.Instance(timerOptions, metricTags)).Returns(() => mockTimer.Object);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            if (exceptionType == null)
            {
                await using (metricsRoot.RecordTime(timerOptions, metricTags, timeUnit, true))
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                stopWatch.Stop();

                stopWatch.ElapsedMilliseconds.Should().BeGreaterOrEqualTo((long) TimeSpan.FromSeconds(1).TotalMilliseconds);

                mockTimer.Verify(s => s.Record(It.IsAny<long>(), TimeUnit.Milliseconds, null));
            }
            else
            {
                try
                {
                    // ReSharper disable once ConvertToUsingDeclaration
                    await using (metricsRoot.RecordTime(timerOptions, metricTags, timeUnit, true))
                    {
                        Assert.True(false, "Should have thrown conversion exception");
                    }
                }
                catch (TimeUnitNotSupportedException ex)
                {
                    ex.Message.Should().Contain(timeUnit.ToString());
                    ex.Message.Should().EndWith("not supported.");
                }
            }
        }
    }
}
