using FluentAssertions;
using System;
using Xunit;

namespace EngineBlox.Timezones.Test.Unit
{
    public class UkDateTimeTests
    {
        [Fact]
        public void TestUkTimeNow()
        {
            var ukNow = UkDateTime.Now;

            var utcNow = DateTime.UtcNow;
            var ukTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcNow, "GMT Standard Time");

            ukNow.Should().BeCloseTo(ukTime);
        }

        [Fact]
        public void GivenItIsWinter_WhenFromUtc_ThenGmtReturned()
        {
            var utcInWinter = "2020-12-25T14:16:14Z";

            var ukTime = UkDateTime.FromUtc(utcInWinter);

            ukTime.ToString("s").Should().Be("2020-12-25T14:16:14");
        }

        [Fact]
        public void GivenItIsSummer_WhenFromUtc_ThenBstReturned()
        {
            var utcInSummer = "2021-06-25T14:16:14Z";

            var ukTime = UkDateTime.FromUtc(utcInSummer);

            ukTime.ToString("s").Should().Be("2021-06-25T15:16:14");
        }
    }
}
