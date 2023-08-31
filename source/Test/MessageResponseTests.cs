﻿// ---------------------------------------------------------------------------- //
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

namespace Finebits.Network.RestClient.Test
{
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
    internal class MessageResponseTests
    {
        [Test]
        public void Send_BadResponse_StringContent_Exception()
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
        public void Send_BadResponse_EmptyStringContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using StringMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.BadRequestEndpoint);

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
        public void Send_BadResponse_JsonContent_Exception()
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
        public void Send_BadResponse_EmptyJsonContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using JsonMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.BadRequestEndpoint);

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
        public void Send_BadResponse_StreamContent_Exception()
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
        public void Send_BadResponse_EmptyStreamContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using StreamMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.BadRequestEndpoint);

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
        public void Send_MethodGet_BadResponse_Headers_Exception()
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
        public void Send_MethodGet_OkResponse_Headers_Success()
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
        public void Send_MethodHead_NoContent_Headers_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using StringMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.HeaderSuccessRequestEndpoint, HttpMethod.Head);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.NoContent));

                Assert.That(message.Response.Content, Is.EqualTo(default(string)));
                Assert.That(message.Response.Headers, Does.Contain(new KeyValuePair<string, IEnumerable<string>>(Mocks.HttpMessageHandlerCreator.TestUri.HeaderKey, new[] { Mocks.HttpMessageHandlerCreator.TestUri.HeaderOkValue, Mocks.HttpMessageHandlerCreator.TestUri.HeaderOkExtraValue })));
            });
        }

        [Test]
        public void Send_OkResponse_HeaderContent_Success()
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

        [Test]
        public void Send_BadResponse_HeaderContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, Mocks.HttpMessageHandlerCreator.TestUri.Host);

            using HeaderMessage message = new(Mocks.HttpMessageHandlerCreator.TestUri.BadRequestEndpoint);

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
