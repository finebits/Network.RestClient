using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Test.Data;

namespace Finebits.Network.RestClient.Test
{
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
    internal class RestClientTests
    {
        [Test]
        public void Construct_NullParam_Exception()
        {
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new TestRestClient(null, null));

                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.ParamName, Is.EqualTo("httpClient"));
            }

            {
                var exception = Assert.Throws<ArgumentNullException>(() => new TestRestClient(null, new Uri("https://any")));

                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.ParamName, Is.EqualTo("httpClient"));
            }
        }

        [Test]
        public void Send_NullParam_Exception()
        {
            using HttpClient httpClient = new();
            TestRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            {
                var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SendMessageAsync(null).ConfigureAwait(false));

                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.ParamName, Is.EqualTo("message"));
            }

            {
                var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SendMessageAsync(null, new CancellationTokenSource().Token).ConfigureAwait(false));

                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.ParamName, Is.EqualTo("message"));
            }
        }

        [Test]
        [TestCaseSource(typeof(Test.Data.HttpStatusCodeTestData), nameof(Test.Data.HttpStatusCodeTestData.ErrorHttpStatusCodeCases))]
        public void Send_UnsuccessfulStatusCode_Exception(HttpStatusCode code)
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            TestRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            Data.TestMessage<StringResponse> message = new(Mocks.HttpMessageHandlerCreator.TestUri.GetHttpStatusCodeEndpoint(code));

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(code));
            });
        }

        [Test]
        [TestCaseSource(typeof(Test.Data.HttpStatusCodeTestData), nameof(Test.Data.HttpStatusCodeTestData.SuccessHttpStatusCodeCases))]
        public void Send_SuccessStatusCode_Success(HttpStatusCode code)
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            TestRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            Data.TestMessage<StringResponse> message = new(Mocks.HttpMessageHandlerCreator.TestUri.GetHttpStatusCodeEndpoint(code));
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;

            Assert.DoesNotThrowAsync(async () => httpStatusCode = await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(httpStatusCode, Is.EqualTo(code));
                Assert.That(message.HttpStatus, Is.EqualTo(code));
            });
        }

        [Test]
        public void Send_RequestCancellationToken_Exception()
        {
            using CancellationTokenSource cts = new();
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.CreateCancellationToken(cts).Object);
            TestRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            Data.TestMessage<StringResponse> message = new(Mocks.HttpMessageHandlerCreator.TestUri.OkEndpoint);

            var exception = Assert.CatchAsync<OperationCanceledException>(async () => await client.SendMessageAsync(message, cts.Token).ConfigureAwait(false));
            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void Send_CancellationToken_Exception()
        {
            using CancellationTokenSource cts = new();
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.CreateCancellationToken(cts).Object);
            TestRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            Data.TestMessage<StringResponse> message = new(Mocks.HttpMessageHandlerCreator.TestUri.OkEndpoint);

            cts.Cancel();

            var exception = Assert.CatchAsync<OperationCanceledException>(async () => await client.SendMessageAsync(message, cts.Token).ConfigureAwait(false));
            Assert.That(exception, Is.Not.Null);
        }
    }
}
