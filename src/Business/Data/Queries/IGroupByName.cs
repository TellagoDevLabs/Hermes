using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface IGroupByName
    {
        bool Exists(string name);
        Group Get(string name);
    }
}