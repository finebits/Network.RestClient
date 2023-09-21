# Network.RestClient: .NET REST Client

![Target](https://img.shields.io/badge/dynamic/xml?label=target&query=//TargetFramework[1]&url=https://raw.githubusercontent.com/finebits/Network.RestClient/main/source/Network.RestClient/Network.RestClient.csproj)
[![License](https://img.shields.io/github/license/finebits/Network.RestClient.svg)](https://github.com/finebits/Network.RestClient/blob/main/LICENSE)
[![NuGet version](https://img.shields.io/nuget/v/Finebits.Network.RestClient.svg?logo=nuget)](https://www.nuget.org/packages/Finebits.Network.RestClient/)
[![Downloads](https://img.shields.io/nuget/dt/Finebits.Network.RestClient)](https://www.nuget.org/packages/Finebits.Network.RestClient)

`Network.RestClient` is a .NET Standard 2.0 C# Library for interaction with REST APIs.

|Branch|Status|Coverage|
|:-|:-:|:-:|
| **[main](https://github.com/finebits/Network.RestClient/tree/main)** | <sub>[![Main branch: build and test](https://img.shields.io/github/actions/workflow/status/finebits/Network.RestClient/build-and-test.yml?branch=main&logo=github&label=)](https://github.com/finebits/Network.RestClient/actions/workflows/build-and-test.yml?query=branch%3Amain)</sub> | <sub>![Test coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Network.RestClient-main-total-test-coverage.json)</sub> |
| **[develop](https://github.com/finebits/Network.RestClient/tree/develop)** | <sub>[![Develop branch: build and test](https://img.shields.io/github/actions/workflow/status/finebits/Network.RestClient/build-and-test.yml?branch=develop&logo=github&label=)](https://github.com/finebits/Network.RestClient/actions/workflows/build-and-test.yml?query=branch%3Adevelop)</sub> | <sub>![Test coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/finebits-github/74f6d448f4f568a286d4622e92afbc75/raw/Network.RestClient-develop-total-test-coverage.json)</sub> |

## Features

- Default HTTP Methods (GET, POST, PUT, HEAD, DELETE, PATCH) support.
- Request and response serialization with System.Text.Json.

## Example Usage

```C#
using Finebits.Network.RestClient;

// Defining a client to interact with the API
class Client : Finebits.Network.RestClient.Client
{

    public Client(HttpClient client)
        : base(client, new Uri("https://host"))
    {}

    public async Task<User> CreateUserAsync(User user)
    {
        var message = new CreateUserMessage();
        await SendAsync(message).ConfigureAwait(false);

        return message.Response.Content;
    }
}
```

```C#
using Finebits.Network.RestClient;

// API request definition.
class CreateUserMessage 
    : Message<JsonResponse<CreateUserMessage.ResponseContent>,
              JsonRequest<CreateUserMessage.User>>
{
    public override Uri Endpoint => new Uri("/user", uriKind: UriKind.Relative);
    public override HttpMethod Method => HttpMethod.Post;

    public struct User
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public struct ResponseContent
    {
        public bool Success { get; set; }
    }
}
```
