using System;
using System.Configuration;
using System.Linq;
using Microsoft.Azure.Documents.Client;

namespace Datastore {
    public class DocumentDbEnvironmentRepository : IEnvironmentRepository
    {
        private readonly DocumentClient _client;
        private readonly Uri _environmentsCollection;
        private readonly FeedOptions _feedOptions;

        public DocumentDbEnvironmentRepository() {
            var configuration = ConfigurationManager.ConnectionStrings;
            var datasource = configuration["Datasource"];

            var connectionString = datasource == null
                ? System.Environment.GetEnvironmentVariable("CONNECTIONSTRING_Datasource")
                : datasource.ConnectionString;

            if (connectionString == null) {
                throw new ConfigurationErrorsException("Unable to get connection string for DocumentDB.");
            }

            var context = connectionString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(key => key.Split('=')[0], value => value.Split(new[] {'='}, 2)[1]);
            _client = new DocumentClient(new Uri(context["AccountEndpoint"]), context["AccountKey"]);

            _feedOptions = new FeedOptions {
                MaxItemCount = 1
            };

            _environmentsCollection = UriFactory.CreateDocumentCollectionUri(context["Database"],
                context["Collection"]);
        }

        public Environment Get(string environment) {
            var collection = _client.CreateDocumentQuery<Environment>(_environmentsCollection, _feedOptions);
            return collection
                .AsEnumerable()
                .FirstOrDefault(x => x.Id.StartsWith(environment, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}