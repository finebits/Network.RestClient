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

namespace Finebits.Network.RestClient.Test.Mocks
{
    internal static class HttpMessageHandlerCreator
    {
        internal static class TestUri
        {
            public static readonly Uri Host = new("https://test-host/");

            public static readonly Uri OkEndpoint = new("/ok", UriKind.Relative);
            public static readonly Uri BadRequestEndpoint = new("/bad-request", UriKind.Relative);

            public static readonly Uri HttpStatusCodeEndpoint = new("/http-status-code", UriKind.Relative);
            public const string HttpStatusCodeQueryParam = "code";

            public static readonly Uri StringPayloadEndpoint = new("/string/payload", UriKind.Relative);
            public static readonly Uri StringOkEndpoint = new("/string/ok", UriKind.Relative);
            public static readonly Uri StringBadRequestEndpoint = new("/string/bad-request", UriKind.Relative);
            public const string StringOkValue = "šomē-ütf8-valúē";
            public const string StringBadRequestValue = "bad-rēqûēšt-ütf8-valūē";

            public static readonly Uri JsonPayloadEndpoint = new("/json/payload", UriKind.Relative);
            public static readonly Uri JsonOkEndpoint = new("/json/ok", UriKind.Relative);
            public static readonly Uri JsonBadRequestEndpoint = new("/json/bad-request", UriKind.Relative);
            public const string JsonOkValue = "šomē-ütf8-valúē";
            public const string JsonErrorValue = "ērror-ùtf8-valūē";
            public const string JsonErrorDescriptionValue = "ērror-dēscrīptїon-ûtf8-valūē";

            public static readonly Uri StreamOkEndpoint = new("/stream/ok", UriKind.Relative);
            public static readonly Uri StreamBadRequestEndpoint = new("/stream/bad-request", UriKind.Relative);
            public const string StreamOkValue = "šomē-ütf8-valúē-和平";
            public const string StreamBadRequestValue = "bad-rēqûēšt-ütf8-valūē-和平";

            public static readonly Uri ContentHeaderOkEndpoint = new("/content-header/ok", UriKind.Relative);
            public static readonly Uri HeaderSuccessRequestEndpoint = new("/header/success", UriKind.Relative);
            public static readonly Uri HeaderBadRequestEndpoint = new("/header/bad-request", UriKind.Relative);

            public const string ContentHeaderKey = "content-header-key";
            public const string ContentHeaderValue = "šomē-ütf8-valúē";

            public const string HeaderKey = "header-key";
            public const string HeaderOkValue = "šomē-ütf8-valúē";
            public const string HeaderOkExtraValue = "šomē-ēxtra-ütf8-valúē";
            public const string HeaderBadRequestValue = "bad-rēqûēšt-ütf8-valūē";

            public static Uri GetHttpStatusCodeEndpoint(HttpStatusCode code)
            {
                return new($"{HttpStatusCodeEndpoint}?{HttpStatusCodeQueryParam}={code}", UriKind.Relative);
            }
        }

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
                    match: (uri) => uri.AbsolutePath.Contains(new Uri(TestUri.Host, TestUri.HttpStatusCodeEndpoint).AbsolutePath, StringComparison.Ordinal),
                    valueFunction: (request) =>
                    {
                        var collection = HttpUtility.ParseQueryString(request?.RequestUri?.Query ?? string.Empty, System.Text.Encoding.Default);
                        string code = collection[TestUri.HttpStatusCodeQueryParam] ?? string.Empty;
                        HttpStatusCode statusCode = Enum.Parse<HttpStatusCode>(code);

                        return new HttpResponseMessage()
                        {
                            StatusCode = statusCode,
                        };
                    }
                )
                .Configure
                (
                    uri: new Uri(TestUri.Host, TestUri.StringPayloadEndpoint),
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
                    uri: new Uri(TestUri.Host, TestUri.JsonPayloadEndpoint),
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
                    uri: new Uri(TestUri.Host, TestUri.StringOkEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(TestUri.StringOkValue)
                    }
                )
                .Configure
                (
                    uri: new Uri(TestUri.Host, TestUri.StringBadRequestEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent(TestUri.StringBadRequestValue)
                    }
                )
                .Configure
                (
                    uri: new Uri(TestUri.Host, TestUri.JsonOkEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = JsonContent.Create(new
                        {
                            value = TestUri.JsonOkValue
                        })
                    }
                )
                .Configure
                (
                    uri: new Uri(TestUri.Host, TestUri.JsonBadRequestEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = JsonContent.Create(new
                        {
                            error = TestUri.JsonErrorValue,
                            error_description = TestUri.JsonErrorDescriptionValue
                        })
                    }
                )
                .Configure
                (
                    uri: new Uri(TestUri.Host, TestUri.StreamOkEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(TestUri.StreamOkValue)))
                    }
                )
                .Configure
                (
                    uri: new Uri(TestUri.Host, TestUri.StreamBadRequestEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(TestUri.StreamBadRequestValue)))
                    }
                )
                .Configure
                (
                    uri: new Uri(TestUri.Host, TestUri.HeaderSuccessRequestEndpoint),
                    valueFunction: (rm) =>
                    {
                        var response = new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.NoContent,
                        };

                        if (rm?.Method != HttpMethod.Head)
                        {
                            response.StatusCode = HttpStatusCode.OK;
                            response.Content = new StringContent(TestUri.StringOkValue);
                        }

                        response.Headers.Add(TestUri.HeaderKey, TestUri.HeaderOkValue);
                        response.Headers.Add(TestUri.HeaderKey, TestUri.HeaderOkExtraValue);

                        return response;
                    }
                )
                .Configure
                (
                    uri: new Uri(TestUri.Host, TestUri.HeaderBadRequestEndpoint),
                    valueFunction: (_) =>
                    {
                        var response = new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Content = new StringContent(TestUri.StringBadRequestValue),
                        };

                        response.Headers.Add(TestUri.HeaderKey, TestUri.HeaderBadRequestValue);

                        return response;
                    }
                )
                .Configure
                (
                    uri: new Uri(TestUri.Host, TestUri.ContentHeaderOkEndpoint),
                    valueFunction: (_) =>
                    {
                        var response = new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(TestUri.StringBadRequestValue),
                        };

                        response.Content.Headers.Add(TestUri.ContentHeaderKey, TestUri.ContentHeaderValue);

                        response.Headers.Add(TestUri.HeaderKey, TestUri.HeaderOkValue);
                        response.Headers.Add(TestUri.HeaderKey, TestUri.HeaderOkExtraValue);

                        return response;
                    }
                )
                .Configure
                (
                    uri: new Uri(TestUri.Host, TestUri.BadRequestEndpoint),
                    valueFunction: (_) => new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest
                    }
                )
                .Configure
                (
                    uri: new Uri(TestUri.Host, TestUri.OkEndpoint),
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
