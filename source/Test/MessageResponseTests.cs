using Finebits.Network.RestClient.Test.Fakes;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

using System.Net;


namespace Finebits.Network.RestClient.Test
{
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
    internal class MessageResponseTests
    {
        [Test]
        public void Send_StringBadResponse_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using StringMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.StringBadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.EqualTo(Mocks.HttpMessageHandlerCreator.TestUri.StringBadRequestValue));
            });
        }

        [Test]
        public void Send_StringOkResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using StringMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.StringOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content, Is.EqualTo(Mocks.HttpMessageHandlerCreator.TestUri.StringOkValue));
            });
        }

        [Test]
        public void Send_JsonBadResponse_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using JsonMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.JsonBadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content.Error, Is.EqualTo(Mocks.HttpMessageHandlerCreator.TestUri.JsonErrorValue));
                Assert.That(message.Response.Content.ErrorDescription, Is.EqualTo(Mocks.HttpMessageHandlerCreator.TestUri.JsonErrorDescriptionValue));
                Assert.That(message.Response.Content.Value, Is.EqualTo(default));
            });
        }

        [Test]
        public void Send_JsonOkResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using JsonMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.JsonOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content.Error, Is.EqualTo(default));
                Assert.That(message.Response.Content.ErrorDescription, Is.EqualTo(default));
                Assert.That(message.Response.Content.Value, Is.EqualTo(Mocks.HttpMessageHandlerCreator.TestUri.JsonOkValue));
            });
        }

        [Test]
        public void Send_StreamBadResponse_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using StreamMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.StreamBadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));

                Assert.That(message.Response.Stream, Is.Not.Null);
                StreamReader reader = new(message.Response.Stream);
                Assert.That(reader.ReadToEnd(), Is.EqualTo(Mocks.HttpMessageHandlerCreator.TestUri.StreamBadRequestValue));
            });
        }

        [Test]
        public void Send_StreamOkResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using StreamMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.StreamOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(message.Response.Stream, Is.Not.Null);
                StreamReader reader = new(message.Response.Stream);
                Assert.That(reader.ReadToEnd(), Is.EqualTo(Mocks.HttpMessageHandlerCreator.TestUri.StreamOkValue));
            });
        }

        [Test]
        public void Send_MethodGet_HeaderBadResponse_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using StringMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.HeaderBadRequestEndpoint, HttpMethod.Get);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));

                Assert.That(message.Response.Content, Is.EqualTo(Mocks.HttpMessageHandlerCreator.TestUri.StringBadRequestValue));
                Assert.That(message.Response.Headers, Does.Contain(new KeyValuePair<string, IEnumerable<string>>(Mocks.HttpMessageHandlerCreator.TestUri.HeaderKey, new[] { Mocks.HttpMessageHandlerCreator.TestUri.HeaderBadRequestValue })));
            });
        }

        [Test]
        public void Send_MethodGet_HeaderResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using StringMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.HeaderSuccessRequestEndpoint, HttpMethod.Get);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(message.Response.Content, Is.EqualTo(Mocks.HttpMessageHandlerCreator.TestUri.StringOkValue));
                Assert.That(message.Response.Headers, Does.Contain(new KeyValuePair<string, IEnumerable<string>>(Mocks.HttpMessageHandlerCreator.TestUri.HeaderKey, new[] { Mocks.HttpMessageHandlerCreator.TestUri.HeaderOkValue, Mocks.HttpMessageHandlerCreator.TestUri.HeaderOkExtraValue })));
            });
        }

        [Test]
        public void Send_MethodHead_HeaderResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using StringMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.HeaderSuccessRequestEndpoint, HttpMethod.Head);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.NoContent));

                Assert.That(message.Response.Content, Is.Empty);
                Assert.That(message.Response.Headers, Does.Contain(new KeyValuePair<string, IEnumerable<string>>(Mocks.HttpMessageHandlerCreator.TestUri.HeaderKey, new[] { Mocks.HttpMessageHandlerCreator.TestUri.HeaderOkValue, Mocks.HttpMessageHandlerCreator.TestUri.HeaderOkExtraValue })));
            });
        }

        [Test]
        public void Send_HeaderContentResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using HeaderMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.ContentHeaderOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(message.Response.GetAllHeaders(), Does.Contain(new KeyValuePair<string, IEnumerable<string>>(Mocks.HttpMessageHandlerCreator.TestUri.HeaderKey, new[] { Mocks.HttpMessageHandlerCreator.TestUri.HeaderOkValue, Mocks.HttpMessageHandlerCreator.TestUri.HeaderOkExtraValue })));
                Assert.That(message.Response.GetAllHeaders(), Does.Contain(new KeyValuePair<string, IEnumerable<string>>(Mocks.HttpMessageHandlerCreator.TestUri.ContentHeaderKey, new[] { Mocks.HttpMessageHandlerCreator.TestUri.ContentHeaderValue })));
            });
        }
    }
}
