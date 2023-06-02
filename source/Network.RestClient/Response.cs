﻿////////////////////////////////////////////////////////////////////////////////
//
//   Copyright 2023 Finebits (https://finebits.com)
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
////////////////////////////////////////////////////////////////////////////////

using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Finebits.Network.RestClient
{
    public class Response
    {
        public HeaderCollection Headers { get; internal set; }
        protected internal virtual Task ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class EmptyResponse : Response
    { }

    public class StringResponse : Response
    {
        public string Content { get; protected set; }

        protected internal override async Task ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            if (content != null)
            {
                Content = await content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }

    public class JsonResponse<TContent> : Response
    {
        public TContent Content { get; protected set; }
        public JsonSerializerOptions Options { get; set; }

        protected internal override async Task ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            if (content != null)
            {
                Content = await content.ReadFromJsonAsync<TContent>(Options, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public class HeadResponse : Response
    {
        public HeaderCollection ContentHeaders { get; protected set; }

        protected internal override Task ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            if (content != null)
            {
                ContentHeaders = new HeaderCollection(content.Headers, false);
            }

            return Task.CompletedTask;
        }

        public HeaderCollection GetAllHeaders()
        {
            return new HeaderCollection(Headers.Concat(ContentHeaders), false);
        }
    }

    public class StreamResponse : Response
    {
        public Stream Stream { get; set; }

        protected internal override Task ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            if (content != null)
            {
                return content.CopyToAsync(Stream);
            }

            return Task.CompletedTask;
        }
    }
}
