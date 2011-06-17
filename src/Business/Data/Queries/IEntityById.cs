using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IEntityById
    {
        bool Exist<TEntity>(Identity id);
        TEntity Get<TEntity>(Identity id) where TEntity : class;
    }
}