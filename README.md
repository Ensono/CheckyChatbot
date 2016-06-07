# Checky Chatbot

Checky is a Slack ChatBot designed to make it easer for us to deliver software
regularly, reliably and repeatable at scale in a natural and sometimes
entertaining way.

## Commands

-   `status`: Retrieve the status of a service, by calling the `/version` and
  `/healthcheck` endpoints to interrogate a service's software version and
  self-reported health.

-   `url`: Returns the set of known URLs for a service, currently will only
  return the Base URI, `/version` and `/healthcheck` endpoints.

-   `usage`: Returns some basic usage information about the chatbot.

-   `cache`: Provides commands to manipulate the cache, very useful if you need
    to invalidate the cache post-haste.

-   `test`: **Experimental** - performs basic HTTP based testing, against an
    environment and service.

## Configuration

### Environment Configuration

For the above commands to work you must 'teach' Checky about each environment,
this is managed through a configuration file for each environment:

```json
{
  "id": "environment-identifier",
  "services": [
    {
      "name": "Service1",
      "baseUri": "https://service1.cloudapp.net"
    },
    {
      "name": "Service2",
      "baseUri": "https://service2.cloudapp.net"
    }
  ]
}
```

It is essential that the environment identifier in the `$.Id` field matches the
filename of the file, all be it with `.json` on the end. In the above example
the file must be named `environment-identifier.json`.

> **TIP**: use the built in `checky-loader` console application to validate
> environment configuration before it is uploaded to Blob Storage.

### Test Configuration

Unlike environments, it is not essential to define tests, however if you wish
to test endpoints you will need to specify at least one *HTTP Test* and the
environments and services it is valid to execute against:

```json
{
  "id": "test-awesome",
  "serviceFilter": [
    "Service1"
  ],
  "environmentFilter": [
    "*"
  ],
  "httpRequestMethod": "POST",
  "httpRequestResource": "/path/to/v1/endpoint",
  "httpRequestBody": "ew0KCSJjb29yZGluYXRlcyI6IHsNCgkJImxhdGl0dWRlIjogNTEuNTM0LA0KICAgICJsb25naXR1ZGUiOiAtMC4xMzgNCiAgICB9LA0KICAiY291bnRyeSI6ICJHQiIsDQp9",
  "httpRequestEncoding": "UTF-8",
  "httpRequestContentType": "application/json",
  "httpRequestHeaders": { },
  "expectHttpResponseCode": "200",
  "expectHttpResponseHeaders":
    {
      "cid": "[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}"
    },
  "expectHttpResponseBodyTokens": [
    {
      "path": "$.county",
      "expectedValue": "London"
    }
  ]
}
```

As this is a fairly sizable document and requires some explanation.  The
document as a whole can be split into two parts **`httpRequest`** and
**`expectHttpResponse`** the two compose the HTTP Request constructed and sent
to the service endpoint and the expectation of the response.

#### HTTP Request - `httpRequest*`

These are constructed as a single HTTP Request, created using the .NET
[`HttpRequestMessage`][msdn-httprequestmessage] and sent asynchronously,
meaning multiple requests may be issued in parallel; additionally they may
start and complete in any order.

-   `httRequestMethod`: the HTTP Method of the outbound request, can be any
    HTTP Method supported by the [`HttpMethod`][msdn-httpmethod] enumeration.

-   `httpRequestResource`: the relative path to the HTTP resource, must be
    relative to the service base URI.

-   `httpResponseBody`: the body of the requests, can be of any format and must
    be Base64 encoded to allow practically any data to be included in the
    request.  Take specical note to align the content of this property with
    the `httpRequestContentType` and `httpRequestEncoding` fields.

-   `httpRequestEncoding`: the encoding of the above `httpResponseBody`, can be
    any Encoding in the [`System.Text` Namespace][msdn-system-text-encoding].

-   `httpRequestContentType`: the content type or content disposition of the
    `httpResponseBody`, can be any content-type supported for the type of
    request being constructed.

-   `httpRequestHeaders`: zero or more headers to include with the request,
    these are simple key-value pairs that together form the headers
    dictionary:

    ```json
    "httpRequestHeaders": {
      "key1": "value1",
      "key2": "value2"
    }
    ```

#### HTTP Resposne `expectHttpResponse*`

The HTTP Response is captured and assertions are made using the content of the
`expectHttpResponse*` elements.  For headers and the message body either
literal strings can be matched, or .NET Regular expressions can be used to
match patterns within the response.

-   `expectHttpResponseCode`: the expected response code as a integer value,
    for example `200` for `OK` or `404` for `File Not Found`.  At the present
    time only literal integers can be used, partial matches are note supported.

-   `expectHttpResponseHeaders`: a dictionary of Key/Value pairs, the key will
    be matched literally, however the value can be a regular expression.  This
    means that if the value contains reserved characters they must be escaped.

-   `expectHttpResponseBodyTokens`: a collection of rules that must be matched
    within the body of the document:

    -   `path`: a JSON path expression that matches exactly one token.

    -   `expectedValue`: a literal string or RegEx that matches the value of
        the token.

### Application Configuration

Checky lazy loads configuration information from `App.config` at runtime when a
connection string is required, if a configuration key is missing from
`App.config`; Checky will attempt to load it from an environment variable.

> **IMPORTANT**: This is an open source project, environmental configuration
> must never be checked in *under any circumstance*.

| `App.config` Setting                                            | DataType |
| --------------------------------------------------------------- | -------- |
| `CacheAbsoluteExpirationTimeSpan_EnvironmentDocument`           | TimeSpan |
| `CacheAbsoluteExpirationTimeSpan_HttpTestDocument`              | TimeSpan |
| `CacheAbsoluteExpirationTimeSpan_IEnumerableOfICloudBlob`       | TimeSpan |
| `CacheAbsoluteExpirationTimeSpan_IEnumerableOfHttpTestDocument` | TimeSpan |

Set's the length of time to cache Environment Documents, HTTP Test and the
lists of blob's that contain them and thus the properties of environments and
tests stored in Blob Storage. Formatted as a .NET TimeSpan and parsed using
[TimeSpan.Parse][msdn-timespan-parse] using an Invariant Culture, i.e.
`hh:mm:ss`.

[msdn-timespan-parse]: https://msdn.microsoft.com/en-us/library/se73z7b9(v=vs.110).aspx
[msdn-httprequestmessage]: https://msdn.microsoft.com/en-us/library/system.net.http.httprequestmessage(v=vs.118).aspx
[msdn-httpmethod]: https://msdn.microsoft.com/en-us/library/system.net.http.httpmethod(v=vs.118).aspx
[msdn-system-text-encoding]: https://msdn.microsoft.com/en-us/library/system.text.encoding.aspx
