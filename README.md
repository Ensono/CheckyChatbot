Checky Chatbot
==============

Checky is a Slack ChatBot designed to make it easer for us to deliver software regularly, reliably and repeatable at scale in a natural and sometimes entertaining way.

Commands
--------

| Command | Description |
| ------- | ----------- |
| status  | Retreve the status of a service, by calling the `/version` and `/healthcheck` endpoints to interrogate a service's software version and self-reported health |
| url     | Returns the set of known URLs for a service, currently will only return the Base URI, `/version` and `/healthcheck` endpoints |

Configuration
-------------

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
