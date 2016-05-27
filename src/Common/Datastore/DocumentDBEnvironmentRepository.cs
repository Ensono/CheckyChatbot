using System;
using System.Linq;
using Configuration;
using Microsoft.Azure.Documents.Client;

namespace Datastore {
    public class DocumentDbEnvironmentRepository : IEnvironmentRepository {
        private readonly DocumentClient _client;
        private readonly Uri _environmentsCollection;
        private readonly FeedOptions _feedOptions;

        public DocumentDbEnvironmentRepository(IConfigurationRepository configuration) {
            var connectionString = configuration.GetConnectionString("Datasource");

            var context = connectionString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(key => key.Split('=')[0], value => value.Split(new[] {'='}, 2)[1]);
            _client = new DocumentClient(new Uri(context["AccountEndpoint"]), context["AccountKey"]);

            _feedOptions = new FeedOptions {
                MaxItemCount = 1
            };

            _environmentsCollection = UriFactory.CreateDocumentCollectionUri(context["Database"],
                context["Collection"]);
        }

        public EnvironmentDocument Get(string environment) {
            var collection = _client.CreateDocumentQuery<EnvironmentDocument>(_environmentsCollection, _feedOptions);
            return collection
                .AsEnumerable()
                .FirstOrDefault(x => x.Id.StartsWith(environment, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}