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

using Moq;
using Moq.Protected;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using System.Web;

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

            public static Uri GetHttpStatusCodeEndpoint(HttpStatusCode code)
            {
                return new($"{HttpStatusCodeEndpoint}?{HttpStatusCodeQueryParam}={code}", UriKind.Relative);
            }
        }

        public static Mock<HttpMessageHandler> CreateSuccess()
        {
            var mock = new Mock<HttpMessageHandler>();

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null && rm.RequestUri.AbsolutePath.EndsWith("token-uri") == true),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = "fake-access-token",
                            token_type = "Bearer",
                            expires_in = 3600,
                            refresh_token = "fake-refresh-token",
                            scope = "email-fake-scope profile-fake-scope",
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.Host.Equals("google", StringComparison.Ordinal)
                                               && rm.RequestUri.AbsolutePath.EndsWith("token-uri") == true),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = "fake-access-token",
                            token_type = "Bearer",
                            expires_in = 3600,
                            refresh_token = "fake-refresh-token",
                            scope = "email-fake-scope profile-fake-scope",
                            id_token = "fake-id-token",
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null && rm.RequestUri.AbsolutePath.EndsWith("refresh-uri") == true),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = "fake-new-access-token",
                            token_type = "Bearer",
                            expires_in = 3600,
                            refresh_token = "fake-new-refresh-token",
                            scope = "email-fake-scope profile-fake-scope",

                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null
                                               && rm.RequestUri.Host.Equals("google", StringComparison.Ordinal)
                                               && rm.RequestUri.AbsolutePath.EndsWith("refresh-uri") == true),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(
                        new
                        {
                            access_token = "fake-access-token",
                            token_type = "Bearer",
                            expires_in = 3600,
                            refresh_token = "",
                            scope = "email-fake-scope profile-fake-scope",
                        }),
                });

            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(rm => rm.RequestUri != null && rm.RequestUri.AbsolutePath.EndsWith("revoke-uri") == true),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(new { })
                });

            return mock;
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
