using EngineBlox.Api.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace EngineBlox.Api.Requests
{
    public enum JsonNamingStrategy { Pascal, Camel }

    public class RequestBuilder
    {
        public static RequestBuilder Default => new RequestBuilder();

        public JsonNamingStrategy NamingStrategy { get; set; } = JsonNamingStrategy.Camel;
        public RequestBuilder UseJsonNamingStrategy(JsonNamingStrategy namingStrategy)
        {
            NamingStrategy = namingStrategy;
            return this;
        }

        public List<Header> Headers { get; set; } = new List<Header>();
        public RequestBuilder AddHeader(string name, string value)
        {
            Headers.Add(new Header(name, value));
            return this;
        }

        public List<UriSegmentParameter> UriSegmentParameters { get; set; } = new List<UriSegmentParameter>();
        public RequestBuilder AddUriSegmentParameter(string name, string value)
        {
            UriSegmentParameters.Add(new UriSegmentParameter(name, value));
            return this;
        }

        public List<QueryParameter> QueryParameters { get; set; } = new List<QueryParameter>();
        public RequestBuilder AddQueryParameter(string name, string value)
        {
            QueryParameters.Add(new QueryParameter(name, value));
            return this;
        }

        public Uri BuildUri(Uri baseAddress, string relativeUri)
        {
            foreach (var uriSegmentParameter in UriSegmentParameters)
            {
                if (!relativeUri.Contains($"{{{uriSegmentParameter.Name}}}"))
                    throw new ApiException($"Invalid request in api. Requested to replace {{{uriSegmentParameter.Name}}} with {uriSegmentParameter.Value} in uri {relativeUri} but {{{uriSegmentParameter.Name}}} is not present");

                relativeUri = relativeUri.Replace($"{{{uriSegmentParameter.Name}}}", WebUtility.UrlEncode(uriSegmentParameter.Value));
            }

            bool queryStringStarted = relativeUri.Contains("?");
            var paramsBuilder = new StringBuilder();

            foreach (var queryParameter in QueryParameters)
            {
                if (queryStringStarted)
                {
                    paramsBuilder.Append("&");
                }
                else
                {
                    paramsBuilder.Append("?");
                    queryStringStarted = true;
                }

                paramsBuilder.Append($"{WebUtility.UrlEncode(queryParameter.Name)}={WebUtility.UrlEncode(queryParameter.Value)}");
            }

            return new Uri(baseAddress, $"{relativeUri}{paramsBuilder}");
        }
    }
}
