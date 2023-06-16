# Network.RestClient: .NET REST Client

`Network.RestClient` is a .NET Standard 2.0 C# Library for interaction with REST APIs.

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