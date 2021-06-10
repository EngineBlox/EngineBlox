namespace EngineBlox.Api.Requests
{
    public class UriSegmentParameter
    {
        public UriSegmentParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
