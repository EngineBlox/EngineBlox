using EngineBlox.Api.Configuration;
using EngineBlox.Api.Exceptions;
using EngineBlox.Test.Utility.Configuration;
using FluentAssertions;
using System;
using Xunit;

namespace EngineBlox.Api.Test.Unit.Configuration
{
    public class ConfigurationExtensionsTests
    {
        [Fact]
        public void GivenConfigurationAvailable_WhenGetValueOrThrow_ThenValueReturned()
        {
            var config = new InMemoryConfigurationBuilder()
                .AddOrUpdateConfiguration("TestValue", "ThisWorks")
                .Build();

            config.GetValueOrThrow("TestValue").Should().Be("ThisWorks");
        }

        [Fact]
        public void GivenConfigurationMissing_WhenGetValueOrThrow_ThenHelpfulError()
        {
            var config = new InMemoryConfigurationBuilder()
                .AddOrUpdateConfiguration("TestValue", "ThisWorks")
                .Build();

            Action getConfig = () => config.GetValueOrThrow("NotPresent");

            getConfig.Should().Throw<ApiException>()
                .WithMessage("NotPresent is not present in configuration or has no value");
        }

        [Fact]
        public void GivenConfigurationEmpty_WhenGetValueOrThrow_ThenHelpfulError()
        {
            var config = new InMemoryConfigurationBuilder()
                .AddOrUpdateConfiguration("EmptyValue", "")
                .Build();

            Action getConfig = () => config.GetValueOrThrow("EmptyValue");

            getConfig.Should().Throw<ApiException>()
                .WithMessage("EmptyValue is not present in configuration or has no value");
        }
    }
}
