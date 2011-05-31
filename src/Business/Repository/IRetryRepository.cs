using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Repository
{
    public interface IRetryRepository
    {
        Retry Create(Retry instance);
        Retry Update(Retry instance);
        Retry Get(Identity id);
        void Delete(Identity id);
        IEnumerable<Retry> Find(string query, int? skip, int? limit);
    }
}