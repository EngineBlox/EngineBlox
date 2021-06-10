using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace EngineBlox.Test.Utility.Configuration
{
    public class InMemoryConfigurationBuilder
    {
        public Dictionary<string, string> Configuration { get; } = new Dictionary<string, string>();
        public InMemoryConfigurationBuilder AddOrUpdateConfiguration(string name, string value)
        {
            if (name is null) throw new TestException($"Cannot add configuration with null name");

            if (Configuration.ContainsKey(name))
            {
                Configuration[name] = value;
            }
            else
            {
                Configuration.Add(name, value);
            }

            return this;
        }

        public IConfigurationRoot Build()
        {
            return new ConfigurationBuilder().AddInMemoryCollection(Configuration.ToList()).Build();
        }
    }
}
