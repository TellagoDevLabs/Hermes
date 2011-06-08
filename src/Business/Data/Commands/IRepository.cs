using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Commads
{
    public interface IRepository<T>
        where T : EntityBase
    {
        void MakePersistent(T entity);
        void MakeTransient(Identity id);
        void Update(T entity);
    }
}