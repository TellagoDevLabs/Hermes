using System;
using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;

namespace Business.Tests.Util
{
    public class StubRepository<T> : IRepository<T> where T : DocumentBase
    {
        public StubRepository(params T[] entities)
        {
            Documents = new HashSet<T>(entities);
            Updates = new HashSet<T>();
        }

        public HashSet<T> Documents { get; set; }

        public HashSet<T> Updates { get; set; }

        public void MakePersistent(T document)
        {
            Documents.Add(document);
        }

        public void MakeTransient(Identity id)
        {
            Documents.Remove(Documents.FirstOrDefault(e => e.Id == id));
        }

        public void Update(T document)
        {
            Updates.Add(document);
        }
    }
}