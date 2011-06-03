using System;
using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace Business.Tests.Util
{
    public class StubCudOperations<T> : ICudOperations<T> where T : DocumentBase
    {
        public StubCudOperations(params T[] entities)
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

        public void MakeTransient(T document)
        {
            Documents.Remove(Documents.FirstOrDefault(e => e.Id == document.Id));
        }

        public void Update(T document)
        {
            Updates.Add(document);
        }
    }
}