# Checky Chatbot

Checky is a Slack ChatBot designed to make it easer for us to deliver software regularly, reliably and repeatable at scale in a natural and sometimes entertaining way.

## Commands

 - `status` - Retreve the status of a service, by calling the `/version` and `/healthcheck` endpoints to interrogate a service's software version and self-reported health.
 - `url` -  Returns the set of known URLs for a service, currently will only return the Base URI, `/version` and `/healthcheck` endpoints.

## Configuration

### Environment Configuration

For the above commands to work you must 'teach' Checky about each environment, this is managed through a configuration file for each environment:

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

It is essential that the environment identifier in the `$.Id` field matches the filename of the file, all be it with `.json` on the end.  In the above example the file must be named `environment-identifier.json`.

> **TIP**: use the built in `checky-loader` console application to validate configuration before it is uploaded to Blob Storage.

### Application Configuration

Checky lazy loads configuration information from `App.config` at runtime when a connection string is required, if a configuration key is missing from `App.config`; Checky will attempt to load it from an environment variable.

> **IMPORTANT**: This is an open source project, environmental configuration must never be checked in _under any circumstance_.

| `App.config` Setting | Environment Variable | Purpose |
|----------------------|----------------------|---------|
| `CacheAbsoluteExpirationTimeSpan_EnvironmentDocument` | `APPSETTING_CacheAbsoluteExpirationTimeSpan_EnvironmentDocument` | Set's the length of time to cache Environment Documents and thus the properties of an environment. Formatted as a .NET TimeSpan and parsed using [TimeSpan.Parse][msdn-timespan-parse] using an Invariant Culture, i.e. `hh:mm:ss` |


  [msdn-timespan-parse]: https://msdn.microsoft.com/en-us/library/se73z7b9(v=vs.110).aspx
