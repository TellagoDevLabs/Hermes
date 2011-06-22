using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.DataAccess.MongoDB.Queries
{
    public class QueryValidator : IQueryValidator
    {
        public bool IsValid(string query)
        {
            try
            {
                query.ToQueryDocument();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}