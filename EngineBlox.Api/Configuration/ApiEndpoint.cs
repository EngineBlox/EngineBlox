namespace EngineBlox.Api.Configuration
{
    public class ApiEndpoint
    {
        public ApiEndpoint(string name, string uri)
        {
            Name = name;
            Uri = uri;
        }

        public string Name { get; }
        public string Uri { get; }
    }
}
