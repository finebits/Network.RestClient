using Finebits.Network.RestClient;
using System.Net;

namespace Test.Data
{
    internal class TestRestClient : Client
    {
        public TestRestClient(HttpClient? httpClient, Uri? baseUri) : base(httpClient, baseUri)
        { }

        public Task<HttpStatusCode> SendMessageAsync(Message? message, CancellationToken cancellationToken = default)
        {
            return SendAsync(message, cancellationToken);
        }
    }
}
