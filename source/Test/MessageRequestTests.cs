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
using System.Net.Mime;
using System.Text;

using Finebits.Network.RestClient.Test.Fakes;

using NUnit.Framework;

using DataSet = Finebits.Network.RestClient.Test.Data.MessageTestData.DataSet;
using UriSet = Finebits.Network.RestClient.Test.Data.MessageTestData.UriSet;

namespace Finebits.Network.RestClient.Test
{
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
    internal class MessageRequestTests
    {
        [Test]
        public void Send_StringPayload_BadResponse_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringPayloadMessage message = new(UriSet.StringPayloadEndpoint)
            {
                StringRequest = new StringRequest
                {
                    Payload = nameof(HttpStatusCode.BadRequest),
                    MediaType = MediaTypeNames.Text.Plain
                }
            };

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.EqualTo(nameof(HttpStatusCode.BadRequest)));
            });
        }

        [Test]
        public void Send_CustomStringPayload_BadResponse_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringPayloadMessage message = new(UriSet.StringPayloadEndpoint)
            {
                StringRequest = new StringRequest
                {
                    Payload = DataSet.EmptyValue,
                    Encoding = Encoding.UTF8,
                    MediaType = MediaTypeNames.Text.Plain
                }
            };

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.EqualTo(DataSet.EmptyValue));
            });
        }

        [Test]
        public void Send_StringPayload_OkResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringPayloadMessage message = new(UriSet.StringPayloadEndpoint)
            {
                StringRequest = new StringRequest
                {
                    Payload = nameof(HttpStatusCode.OK),
                    Encoding = Encoding.UTF8,
                }
            };

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content, Is.EqualTo(nameof(HttpStatusCode.OK)));
            });
        }

        [Test]
        public void Send_JsonPayload_BadResponse_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonPayloadMessage message = new(UriSet.JsonPayloadEndpoint)
            {
                Payload = new JsonPayloadMessage.RequestPayload
                {
                    Code = nameof(HttpStatusCode.BadRequest),
                    Value = DataSet.EmptyValue
                }
            };

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content.Value, Is.EqualTo(DataSet.EmptyValue));
            });
        }

        [Test]
        public void Send_JsonContent_OkResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonPayloadMessage message = new(UriSet.JsonPayloadEndpoint)
            {
                Payload = new JsonPayloadMessage.RequestPayload
                {
                    Code = nameof(HttpStatusCode.OK),
                    Value = DataSet.CustomValue
                }
            };

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content.Value, Is.EqualTo(DataSet.CustomValue));
            });
        }

        [Test]
        public void Send_CustomHeader_OkResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using HeaderMessage message = new(UriSet.CustomHeaderEndpoint)
            {
                Headers = new HeaderCollection(new (string, string)[]
                {
                    (DataSet.HeaderKey, DataSet.CustomValue),
                    (DataSet.HeaderKey, DataSet.ExtraCustomValue),
                })
            };

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Headers, Does.Contain(new KeyValuePair<string, IEnumerable<string>>(DataSet.HeaderKey, new[] { DataSet.CustomValue, DataSet.ExtraCustomValue })));
            });
        }
    }
}
