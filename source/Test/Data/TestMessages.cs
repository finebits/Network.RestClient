namespace Finebits.Network.RestClient.Test.Data
{
    internal class TestMessage<TResponse> : Message<TResponse>
        where TResponse : Response, new()
    {
        public override Uri Endpoint { get; }

        public override HttpMethod Method { get; }

        public TestMessage(Uri endpoint, HttpMethod? method = null)
        {
            Endpoint = endpoint;
            Method = method ?? HttpMethod.Get;
        }
    }
}
