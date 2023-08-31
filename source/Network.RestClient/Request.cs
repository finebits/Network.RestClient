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

using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Finebits.Network.RestClient
{
    public class Request
    {
        public HeaderCollection Headers { get; set; }

        protected internal virtual Task<HttpContent> CreateContentAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<HttpContent>(null);
        }
    }

    public class EmptyRequest : Request
    { }

    public class StringRequest : Request
    {
        public string Payload { get; set; }
        public Encoding Encoding { get; set; }
        public string MediaType { get; set; }

        protected internal override Task<HttpContent> CreateContentAsync(CancellationToken cancellationToken)
        {
            if (Encoding is null)
            {
                return Task.FromResult<HttpContent>(new StringContent(Payload));
            }

            if (MediaType is null)
            {
                return Task.FromResult<HttpContent>(new StringContent(Payload, Encoding));
            }

            return Task.FromResult<HttpContent>(new StringContent(Payload, Encoding, MediaType));
        }
    }

    public class JsonRequest<TJsonPayload> : Request
    {
        public TJsonPayload Payload { get; set; }
        public JsonSerializerOptions Options { get; set; }

        protected internal override Task<HttpContent> CreateContentAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<HttpContent>(JsonContent.Create(inputValue: Payload, options: Options));
        }
    }
}
