using System.Collections.Generic;

namespace TellagoStudios.Hermes.Business.Queries
{
    public interface IGenericJsonPagedQuery
    {
        IEnumerable<T> Execute<T>(string query, int? skip, int? take);
    }
}