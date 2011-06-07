using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IEntityById
    {
        bool Exist<TDocument>(Identity id);
        TDocument Get<TDocument>(Identity id) where TDocument : class;
    }
}