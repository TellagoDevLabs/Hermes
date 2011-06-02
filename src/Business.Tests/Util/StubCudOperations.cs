using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Queries;

namespace Business.Tests.Util
{
    public class StubCudOperations<T> : ICudOperations<T> where T : DocumentBase
    {
        public StubCudOperations()
        {
            Entities = new HashSet<T>();
            Updates = new HashSet<T>();
        }

        public HashSet<T> Entities { get; set; }

        public HashSet<T> Updates { get; set; }

        public void MakePersistent(T document)
        {
            Entities.Add(document);
        }

        public void MakeTransient(T document)
        {
            Entities.Remove(document);
        }

        public void Update(T document)
        {
            Updates.Add(document);
        }
    }
}