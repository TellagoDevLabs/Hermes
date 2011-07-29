using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Data.Queries
{
    public interface ITopicByName
    {
        bool Exists(string name, Identity? groupId);
        Topic Get(string name, Identity? groupId);
    }
}