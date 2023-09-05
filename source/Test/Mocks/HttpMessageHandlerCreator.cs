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

using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using System.Web;

using Finebits.Network.RestClient.Test.Fakes;

using Moq;
using Moq.Protected;

using DataSet = Finebits.Network.RestClient.Test.Data.MessageTestData.DataSet;
using UriSet = Finebits.Network.RestClient.Test.Data.MessageTestData.UriSet;

namespace Finebits.Network.RestClient.Test.Mocks
{
    internal static class HttpMessageHandlerCreator
    {
        public static Mock<HttpMessageHandler> CreateCancellationToken(CancellationTokenSource cts)
        {
            return new Mock<HttpMessageHandler>()
                .Configure
                (
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                    },
                    callback: cts.Cancel
                );
        }

        internal static Mock<HttpMessageHandler> Create()
        {
            return new Mock<HttpMessageHandler>()
                .Configure
                (
                    match: (uri) => uri.AbsolutePath.Contains(new Uri(UriSet.Host, UriSet.HttpStatusCodeEndpoint).AbsolutePath, StringComparison.Ordinal),
                    valueFunction: (request) =>
                    {
                        var collection = HttpUtility.ParseQueryString(request?.RequestUri?.Query ?? string.Empty, System.Text.Encoding.Default);
                        string code = collection[UriSet.HttpStatusCodeQueryParam] ?? string.Empty;
                        HttpStatusCode statusCode = Enum.Parse<HttpStatusCode>(code);

                        return new HttpResponseMessage()
                        {
                            StatusCode = statusCode,
                        };
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.StringPayloadEndpoint),
                    valueFunction: (request) =>
                    {
                        if (request?.Content is StringContent content)
                        {
                            var text = content.ReadAsStringAsync().Result;
                            var success = Enum.TryParse<HttpStatusCode>(text, out var code);

                            return new HttpResponseMessage()
                            {
                                Content = new StringContent(text),
                                StatusCode = success ? code : HttpStatusCode.BadRequest
                            };
                        }

                        return new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.JsonPayloadEndpoint),
                    valueFunction: (request) =>
                    {
                        if (request?.Content is JsonContent content)
                        {
                            var payload = content.ReadFromJsonAsync<JsonPayloadMessage.RequestPayload>().Result;
                            var success = Enum.TryParse<HttpStatusCode>(payload.Code, out var code);

                            return new HttpResponseMessage()
                            {
                                Content = JsonContent.Create(new JsonPayloadMessage.ResponseContent
                                {
                                    Value = payload.Value
                                }),
                                StatusCode = success ? code : HttpStatusCode.BadRequest
                            };
                        }

                        return new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.FormUrlEncodedPayloadEndpoint),
                    valueFunction: (request) =>
                    {
                        if (request?.Content is FormUrlEncodedContent content)
                        {
                            var query = content.ReadAsStringAsync().Result;
                            var collection = HttpUtility.ParseQueryString(query);

                            var success = Enum.TryParse<HttpStatusCode>(collection[DataSet.CodeKey], out var code);

                            return new HttpResponseMessage()
                            {
                                Content = new StringContent(query),
                                StatusCode = success ? code : HttpStatusCode.BadRequest
                            };
                        }

                        return new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.CustomHeaderEndpoint),
                    valueFunction: (request) =>
                    {
                        var response = new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                        };

                        if (request is not null && request?.Headers.TryGetValues(DataSet.HeaderKey, out var values) == true && values is not null)
                        {
                            response.Headers.Add(DataSet.HeaderKey, values);
                            response.StatusCode = HttpStatusCode.OK;
                        }

                        return response;
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.StringOkEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(DataSet.OkValue)
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.StringBadRequestEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent(DataSet.BadRequestValue)
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.JsonOkEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = JsonContent.Create(new
                        {
                            value = DataSet.JsonOkValue
                        })
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.JsonBadRequestEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = JsonContent.Create(new
                        {
                            error = DataSet.JsonErrorValue,
                            error_description = DataSet.JsonErrorDescriptionValue
                        })
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.StreamOkEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(DataSet.OkValue)))
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.StreamBadRequestEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(DataSet.BadRequestValue)))
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.HeaderSuccessRequestEndpoint),
                    valueFunction: (rm) =>
                    {
                        var response = new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.NoContent,
                        };

                        if (rm?.Method != HttpMethod.Head)
                        {
                            response.StatusCode = HttpStatusCode.OK;
                            response.Content = new StringContent(DataSet.OkValue);
                        }

                        response.Headers.Add(DataSet.HeaderKey, DataSet.HeaderOkValue);
                        response.Headers.Add(DataSet.HeaderKey, DataSet.HeaderOkExtraValue);

                        return response;
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.HeaderBadRequestEndpoint),
                    valueFunction: (_) =>
                    {
                        var response = new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Content = new StringContent(DataSet.BadRequestValue),
                        };

                        response.Headers.Add(DataSet.HeaderKey, DataSet.HeaderBadRequestValue);

                        return response;
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.ContentHeaderOkEndpoint),
                    valueFunction: (_) =>
                    {
                        var response = new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(DataSet.BadRequestValue),
                        };

                        response.Content.Headers.Add(DataSet.ContentHeaderKey, DataSet.ContentHeaderValue);

                        response.Headers.Add(DataSet.HeaderKey, DataSet.HeaderOkValue);
                        response.Headers.Add(DataSet.HeaderKey, DataSet.HeaderOkExtraValue);

                        return response;
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.BadRequestEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest
                    }
                )
                .Configure
                (
                    uri: new Uri(UriSet.Host, UriSet.OkEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK
                    }
                );
        }

        private static Mock<HttpMessageHandler> Configure(this Mock<HttpMessageHandler> mock, Func<HttpRequestMessage?, HttpResponseMessage> valueFunction, Expression? expression = null, Action? callback = null)
        {
            expression ??= ItExpr.IsAny<HttpRequestMessage>();

            HttpRequestMessage? httpRequestMessage = null;

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    expression,
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
                {
                    httpRequestMessage = request;
                    callback?.Invoke();
                })
                .ReturnsAsync(() => valueFunction.Invoke(httpRequestMessage));

            return mock;
        }

        private static Mock<HttpMessageHandler> Configure(this Mock<HttpMessageHandler> mock, Func<HttpRequestMessage?, HttpResponseMessage> valueFunction, Uri uri, Action? callback = null)
        {
            Expression expression = ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null && rm.RequestUri.Equals(uri));

            return mock.Configure(valueFunction, expression, callback);
        }

        private static Mock<HttpMessageHandler> Configure(this Mock<HttpMessageHandler> mock, Func<HttpRequestMessage?, HttpResponseMessage> valueFunction, Func<Uri, bool> match, Action? callback = null)
        {
            Expression expression = ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null && match(rm.RequestUri));

            return mock.Configure(valueFunction, expression, callback);
        }
    }
}
