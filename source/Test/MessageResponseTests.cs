// ---------------------------------------------------------------------------- //
//                                                                              //
//   Copyright 2023 Finebits (https://finebits.com/)                            //
//                                                                              //
//   Licensed under the Apache License, Version 2.0 (the "License"),            //
//   you may not use this file except in compliance with the License.           //
//   You may obtain a copy of the License at                                    //
//                                                                              //
//       http://www.apache.org/licenses/LICENSE-2.0                             //
//                                                                              //
//   Unless required by applicable law or agreed to in writing, software        //
//   distributed under the License is distributed on an "AS IS" BASIS,          //
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.   //
//   See the License for the specific language governing permissions and        //
//   limitations under the License.                                             //
//                                                                              //
// ---------------------------------------------------------------------------- //

using System.Diagnostics.CodeAnalysis;
using System.Net;

using Finebits.Network.RestClient.Test.Fakes;

using NUnit.Framework;

using DataSet = Finebits.Network.RestClient.Test.Data.MessageTestData.DataSet;
using UriSet = Finebits.Network.RestClient.Test.Data.MessageTestData.UriSet;

namespace Finebits.Network.RestClient.Test
{
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
    internal class MessageResponseTests
    {
        [Test]
        public void Construct_StreamResponse_Success()
        {
            Assert.DoesNotThrow(() => { using var streamResponse = new StreamResponse(); });
            Assert.DoesNotThrow(() => { using var streamResponse = new StreamResponse(new MemoryStream()); });
        }

        [Test]
        public void Construct_StreamResponse_NullParam_Exception()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => { using var streamResponse = new StreamResponse(null); });

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.ParamName, Is.EqualTo("stream"));
        }

        [Test]
        public void Dispose_StreamResponse_Success()
        {
            Assert.DoesNotThrow(() => { using var streamResponse = new StreamResponse(); streamResponse.Stream.Dispose(); });
            Assert.DoesNotThrow(() => { using var streamResponse = new StreamResponse(); streamResponse.Dispose(); });
        }

        [Test]
        public void Send_BadResponse_StringContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.StringBadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.EqualTo(DataSet.StringBadRequestValue));
            });
        }

        [Test]
        public void Send_BadResponse_EmptyStringContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.BadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.EqualTo(default(string)));
            });
        }

        [Test]
        public void Send_OkResponse_StringContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.StringOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content, Is.EqualTo(DataSet.StringOkValue));
            });
        }

        [Test]
        public void Send_BadResponse_JsonContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonMessage message = new(UriSet.JsonBadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content.Error, Is.EqualTo(DataSet.JsonErrorValue));
                Assert.That(message.Response.Content.ErrorDescription, Is.EqualTo(DataSet.JsonErrorDescriptionValue));
                Assert.That(message.Response.Content.Value, Is.EqualTo(default));
            });
        }

        [Test]
        public void Send_BadResponse_EmptyJsonContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonMessage message = new(UriSet.BadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.EqualTo(default(JsonMessage.Data)));
            });
        }

        [Test]
        public void Send_OkResponse_JsonContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonMessage message = new(UriSet.JsonOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content.Error, Is.EqualTo(default));
                Assert.That(message.Response.Content.ErrorDescription, Is.EqualTo(default));
                Assert.That(message.Response.Content.Value, Is.EqualTo(DataSet.JsonOkValue));
            });
        }

        [Test]
        public void Send_BadResponse_StreamContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StreamMessage message = new(UriSet.StreamBadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));

                Assert.That(message.Response.Stream, Is.Not.Null);
                StreamReader reader = new(message.Response.Stream);
                Assert.That(reader.ReadToEnd(), Is.EqualTo(DataSet.StreamBadRequestValue));
            });
        }

        [Test]
        public void Send_BadResponse_EmptyStreamContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StreamMessage message = new(UriSet.BadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));

                Assert.That(message.Response.Stream, Is.Not.Null);
                Assert.That(message.Response.Stream.Length, Is.Zero);
            });
        }

        [Test]
        public void Send_OkResponse_StreamContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StreamMessage message = new(UriSet.StreamOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(message.Response.Stream, Is.Not.Null);
                StreamReader reader = new(message.Response.Stream);
                Assert.That(reader.ReadToEnd(), Is.EqualTo(DataSet.StreamOkValue));
            });
        }

        [Test]
        public void Send_MethodGet_BadResponse_Headers_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.HeaderBadRequestEndpoint, HttpMethod.Get);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));

                Assert.That(message.Response.Content, Is.EqualTo(DataSet.StringBadRequestValue));
                Assert.That(message.Response.Headers, Does.Contain(new KeyValuePair<string, IEnumerable<string>>(DataSet.HeaderKey, new[] { DataSet.HeaderBadRequestValue })));
            });
        }

        [Test]
        public void Send_MethodGet_OkResponse_Headers_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.HeaderSuccessRequestEndpoint, HttpMethod.Get);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(message.Response.Content, Is.EqualTo(DataSet.StringOkValue));
                Assert.That(message.Response.Headers, Does.Contain(
                    new KeyValuePair<string, IEnumerable<string>>(DataSet.HeaderKey, new[] { DataSet.HeaderOkValue, DataSet.HeaderOkExtraValue }))
                    );
            });
        }

        [Test]
        public void Send_MethodHead_NoContent_Headers_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.HeaderSuccessRequestEndpoint, HttpMethod.Head);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.NoContent));

                Assert.That(message.Response.Content, Is.EqualTo(default(string)));
                Assert.That(message.Response.Headers, Does.Contain(
                    new KeyValuePair<string, IEnumerable<string>>(DataSet.HeaderKey, new[] { DataSet.HeaderOkValue, DataSet.HeaderOkExtraValue }))
                    );
            });
        }

        [Test]
        public void Send_OkResponse_HeaderContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using HeaderMessage message = new(UriSet.ContentHeaderOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(message.Response.GetAllHeaders(), Does.Contain(new KeyValuePair<string, IEnumerable<string>>(DataSet.HeaderKey, new[] { DataSet.HeaderOkValue, DataSet.HeaderOkExtraValue })));
                Assert.That(message.Response.GetAllHeaders(), Does.Contain(new KeyValuePair<string, IEnumerable<string>>(DataSet.ContentHeaderKey, new[] { DataSet.ContentHeaderValue })));
            });
        }

        [Test]
        public void Send_BadResponse_HeaderContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using HeaderMessage message = new(UriSet.BadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));

                Assert.That(message.Response.Headers, Is.Empty);
                Assert.That(message.Response.ContentHeaders, Does.Contain(new KeyValuePair<string, IEnumerable<string>>("Content-Length", new[] { "0" })));
            });
        }
    }
}
