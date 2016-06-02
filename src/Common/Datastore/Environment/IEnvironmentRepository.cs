using System.Collections.Generic;

namespace Checky.Common.Datastore.Environment {
    public interface IEnvironmentRepository {
        IEnumerable<string> Find(string environmentStartsWith);
        EnvironmentDocument Get(string environment);
    }
}