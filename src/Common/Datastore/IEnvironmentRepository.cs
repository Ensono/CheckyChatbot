namespace Datastore {
    public interface IEnvironmentRepository {
        EnvironmentDocument Get(string environment);
    }
}