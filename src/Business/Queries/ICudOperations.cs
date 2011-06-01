namespace TellagoStudios.Hermes.Business.Queries
{
    public interface ICudOperations<in T>
    {
        void MakePersistent(T entity);
        void MakeTransient(T entity);
        void Update(T entity);
    }
}