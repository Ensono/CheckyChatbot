using System.Collections.Generic;

namespace Datastore {
    public interface IEnvironmentRepository {
        IEnumerable<string> Find(string environmentStartsWith);
        EnvironmentDocument Get(string environment);
    }
}