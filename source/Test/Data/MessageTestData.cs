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

using System.Net;

namespace Finebits.Network.RestClient.Test.Data
{
    internal class MessageTestData
    {
        internal static class UriSet
        {
            public static readonly Uri Host = new("https://test-host/");

            public static readonly Uri OkEndpoint = new("/ok", UriKind.Relative);
            public static readonly Uri BadRequestEndpoint = new("/bad-request", UriKind.Relative);

            public static readonly Uri HttpStatusCodeEndpoint = new("/http-status-code", UriKind.Relative);

            public static readonly Uri StringPayloadEndpoint = new("/string/payload", UriKind.Relative);
            public static readonly Uri StringOkEndpoint = new("/string/ok", UriKind.Relative);
            public static readonly Uri StringBadRequestEndpoint = new("/string/bad-request", UriKind.Relative);


            public static readonly Uri JsonPayloadEndpoint = new("/json/payload", UriKind.Relative);
            public static readonly Uri JsonOkEndpoint = new("/json/ok", UriKind.Relative);
            public static readonly Uri JsonBadRequestEndpoint = new("/json/bad-request", UriKind.Relative);

            public static readonly Uri StreamOkEndpoint = new("/stream/ok", UriKind.Relative);
            public static readonly Uri StreamBadRequestEndpoint = new("/stream/bad-request", UriKind.Relative);

            public static readonly Uri ContentHeaderOkEndpoint = new("/content-header/ok", UriKind.Relative);
            public static readonly Uri HeaderSuccessRequestEndpoint = new("/header/success", UriKind.Relative);
            public static readonly Uri HeaderBadRequestEndpoint = new("/header/bad-request", UriKind.Relative);
            public static readonly Uri CustomHeaderEndpoint = new("/header/custom", UriKind.Relative);

            public const string HttpStatusCodeQueryParam = "code";

            public static Uri GetHttpStatusCodeEndpoint(HttpStatusCode code)
            {
                return new($"{HttpStatusCodeEndpoint}?{HttpStatusCodeQueryParam}={code}", UriKind.Relative);
            }
        }

        internal static class DataSet
        {
            public const string StringOkValue = "šomē-ütf8-valúē-你好世界";
            public const string StringBadRequestValue = "bad-rēqûēšt-ütf8-valūē-你好世界";

            public const string JsonOkValue = "šomē-ütf8-valúē-你好世界";
            public const string JsonErrorValue = "ērror-ùtf8-valūē-你好世界";
            public const string JsonErrorDescriptionValue = "ērror-dēscrīptїon-ûtf8-valūē-你好世界";

            public const string StreamOkValue = "šomē-ütf8-valúē-你好世界";
            public const string StreamBadRequestValue = "bad-rēqûēšt-ütf8-valūē-你好世界";

            public const string ContentHeaderKey = "content-header-key";
            public const string ContentHeaderValue = "šomē-ütf8-valúē-你好世界";

            public const string HeaderKey = "header-key";
            public const string HeaderOkValue = "šomē-ütf8-valúē";
            public const string HeaderOkExtraValue = "šomē-ēxtra-ütf8-valúē";
            public const string HeaderBadRequestValue = "bad-rēqûēšt-ütf8-valūē";

            public const string CustomValue = "šomē-ütf8-valúe-你好世界";
            public const string ExtraCustomValue = "šomē-ēxtra-ütf8-valúē-你好世界";
            public const string EmptyValue = "";
        }
    }
}
