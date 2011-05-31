using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Common.Model;

namespace TellagoStudios.Hermes.Common.Repository
{
    public interface IRetryRepository
    {
        Retry Create(Retry instance);
        Retry Update(Retry instance);
        Retry Get(Guid id);
        void Delete(Guid id);
        IEnumerable<Retry> Find(string query, int? skip, int? limit);
    }
}