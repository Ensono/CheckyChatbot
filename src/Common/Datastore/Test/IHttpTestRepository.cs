using System.Collections.Generic;

namespace Checky.Common.Datastore.Test {
    public interface IHttpTestRepository {
        IEnumerable<string> Find(string environment = null, string service = null);
        IEnumerable<HttpTestDocument> GetAll(IEnumerable<string> ids);
    }
}