using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Queries
{
    public interface ICudOperations<T>
        where T : DocumentBase
    {
        void MakePersistent(T document);
        void MakeTransient(T document);
        void Update(T document);
    }
}