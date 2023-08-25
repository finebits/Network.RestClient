using System.Net;

namespace Finebits.Network.RestClient.Test.Fakes
{
    internal class FakeRestClient : Client
    {
        public FakeRestClient(HttpClient? httpClient, Uri? baseUri) : base(httpClient, baseUri)
        { }

        public Task<HttpStatusCode> SendMessageAsync(Message? message, CancellationToken cancellationToken = default)
        {
            return SendAsync(message, cancellationToken);
        }
    }
}
