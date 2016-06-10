# Checky Chatbot

Checky is a Slack chatbot designed to make it easier for us to deliver software
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

-   `test`: **Experimental** - performs basic HTTP-based testing, against an
    environment and service.

## Configuration

### Environment Configuration

For the above commands to work, you must 'teach' Checky about each
environment; declare a collection of environments through one or more
environment definition files:

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

It is essential that the environment identifier in the `$.Id` field matches
the filename of the file; all be it with `.json` on the end. In the above
example, the file must be named `environment-identifier.json`.

> **TIP**: use the built in `checky-loader` console application to validate
> environment configuration in advance of uploading each file to Blob Storage.

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

As this is a relatively sizable document and requires some explanation.  The
document as a whole is divided into two parts **`httpRequest`** and
**`expectHttpResponse`** the two compose the HTTP Request constructed and sent
to the service endpoint and the expectation of the response.

#### HTTP Request - `httpRequest*`

The collection of fields starting with `httpRequest` are used to create a
single HTTP Request, set up using the .NET
[`HttpRequestMessage`][msdn-httprequestmessage] and sent asynchronously,
meaning multiple requests may be issued in parallel; additionally, they may
start and complete in any order.

-   `httRequestMethod`: the HTTP Method of the outbound request can be any
    HTTP Method supported by the [`HttpMethod`][msdn-httpmethod] enumeration.

-   `httpRequestResource`: the relative path to the HTTP resource must be
    About the service base URI.

-   `httpResponseBody`: the body of the requests can be of any format and must
    be Base64 encoded to allow practically any data in the request.  Take
    especial note to align the content of this property with the
    `httpRequestContentType` and `httpRequestEncoding` fields.

-   `httpRequestEncoding`: the encoding of the above `httpResponseBody`, can be
    any Encoding in the [`System.Text` Namespace][msdn-system-text-encoding].

-   `httpRequestContentType`: the content type or content disposition of the
    `httpResponseBody`, can be any content-type supported for the type of
    request.

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

The HTTP Response is captured, and assertions are made using the content of the
`expectHttpResponse*` elements.  For headers and the message body either
literal strings can be matched, or .NET Regular Expressions can be used to
match patterns in the response.

-   `expectHttpResponseCode`: the expected response code as a integer value,
    for example `200` for `OK` or `404` for `File Not Found`.  At present only
    literal integers can be used, partial matches are note supported.

-   `expectHttpResponseHeaders`: a dictionary of Key/Value pairs, the key will
    be matched literally; however the value can be a regular expression.  This
    means that if the value contains reserved characters, escape them.

-   `expectHttpResponseBodyTokens`: a collection of rules all of which must
    match within the body of the document:

    -   `path`: a JSON path expression that matches exactly one token.

    -   `expectedValue`: a literal string or regular expression that matches
        the value of the token.

### Application Configuration

Checky lazy loads configuration information from `App.config` at runtime when a
connection string is required, if a configuration key is missing from
`App.config`; Checky will attempt to load it from an environment variable.

> **IMPORTANT**: This is an open source project, never include environmental
> configuration in the repository *under any circumstance*.

#### Caching

| `App.config` Setting                                            | DataType |
| --------------------------------------------------------------- | -------- |
| `CacheAbsoluteExpirationTimeSpan_EnvironmentDocument`           | TimeSpan |
| `CacheAbsoluteExpirationTimeSpan_HttpTestDocument`              | TimeSpan |
| `CacheAbsoluteExpirationTimeSpan_IEnumerableOfICloudBlob`       | TimeSpan |
| `CacheAbsoluteExpirationTimeSpan_IEnumerableOfHttpTestDocument` | TimeSpan |

Set's the length of time to cache Environment Documents, HTTP Test and the
lists of blob's that contain them and thus the properties of environments and
tests stored in Blob Storage.  Formatted as a .NET TimeSpan and parsed using
[TimeSpan.Parse][msdn-timespan-parse] using an Invariant Culture, i.e.
`hh:mm:ss`.  If these settings are missing from `App.config` then the
application will attempt to load them from Environment Variables with the same
configuration name prefixed with `APPSETTING_`:

-   `APPSETTING_CacheAbsoluteExpirationTimeSpan_EnvironmentDocument`
-   `APPSETTING_CacheAbsoluteExpirationTimeSpan_HttpTestDocument`
-   `APPSETTING_CacheAbsoluteExpirationTimeSpan_IEnumerableOfICloudBlob`
-   `APPSETTING_CacheAbsoluteExpirationTimeSpan_IEnumerableOfHttpTestDocument`

#### Connection Strings

| `App.config` Setting | DataType |
| -------------------- | -------- |
| `EnvironmentsStore`  | Uri      |
| `HttpTestsStore`     | Uri      |

Set's the URI for the Environments and Http Tests Azure Blob Storage container
respectively.  These URIs must be SAS Token based URIs to the blob storage
containers, for convenience.  Generate the appropriately formatted URIs with
the `checky-loader` console application.

[msdn-timespan-parse]: https://msdn.microsoft.com/en-us/library/se73z7b9(v=vs.110).aspx
[msdn-httprequestmessage]: https://msdn.microsoft.com/en-us/library/system.net.http.httprequestmessage(v=vs.118).aspx
[msdn-httpmethod]: https://msdn.microsoft.com/en-us/library/system.net.http.httpmethod(v=vs.118).aspx
[msdn-system-text-encoding]: https://msdn.microsoft.com/en-us/library/system.text.encoding.aspx
