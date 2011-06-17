using System;
using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;

namespace Business.Tests.Util
{
    public class StubRepository<T> : IRepository<T> where T : EntityBase
    {
        public StubRepository(params T[] entities)
        {
            Entities = new HashSet<T>(entities);
            Updates = new HashSet<T>();
        }

        public HashSet<T> Entities { get; set; }

        public HashSet<T> Updates { get; set; }

        public void MakePersistent(T entity)
        {
            Entities.Add(entity);
        }

        public void MakeTransient(Identity id)
        {
            Entities.Remove(Entities.FirstOrDefault(e => e.Id == id));
        }

        public void Update(T entity)
        {
            Updates.Add(entity);
        }
    }
}