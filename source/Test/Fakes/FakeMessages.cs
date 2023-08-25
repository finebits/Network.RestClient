using System.Text.Json.Serialization;

namespace Finebits.Network.RestClient.Test.Fakes
{
    internal class FakeMessage<TResponse> : Message<TResponse>
        where TResponse : Response, new()
    {
        public override Uri Endpoint { get; }

        public override HttpMethod Method { get; }

        public FakeMessage(Uri endpoint, HttpMethod? method = null)
        {
            Endpoint = endpoint;
            Method = method ?? HttpMethod.Get;
        }
    }

    internal class StringMessage : FakeMessage<StringResponse>
    {
        public StringMessage(Uri endpoint, HttpMethod? method = null) : base(endpoint, method)
        { }
    }

    internal class JsonMessage : FakeMessage<JsonResponse<JsonMessage.Data>>
    {
        public JsonMessage(Uri endpoint, HttpMethod? method = null) : base(endpoint, method)
        { }

        public readonly struct Data
        {
            [JsonInclude]
            [JsonPropertyName("error")]
            public string? Error { get; init; }

            [JsonInclude]
            [JsonPropertyName("error_description")]
            public string? ErrorDescription { get; init; }

            [JsonInclude]
            [JsonPropertyName("value")]
            public string? Value { get; init; }
        }
    }

    internal class StreamMessage : CommonMessage<StreamResponse, EmptyRequest>
    {
        public override Uri Endpoint { get; }

        public override HttpMethod Method { get; }

        public StreamMessage(Uri endpoint, HttpMethod? method = null)
        {
            Endpoint = endpoint;
            Method = method ?? HttpMethod.Get;
        }

        protected override EmptyRequest CreateRequest()
        {
            return new EmptyRequest();
        }

        protected override StreamResponse CreateResponse()
        {
            return new StreamResponse(new MemoryStream());
        }
    }

    internal class HeaderMessage : FakeMessage<HeadResponse>
    {
        public HeaderMessage(Uri endpoint, HttpMethod? method = null) : base(endpoint, method)
        { }
    }
}
