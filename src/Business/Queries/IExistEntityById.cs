using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Queries
{
    public interface IExistEntityById
    {
        bool Execute<TCollection>(Identity id);
    }
}