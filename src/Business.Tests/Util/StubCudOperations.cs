using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Queries;

namespace Business.Tests.Util
{
    public class StubCudOperations<T> : ICudOperations<T>
    {
        public StubCudOperations()
        {
            Entities = new HashSet<T>();
            Updates = new HashSet<T>();
        }

        public HashSet<T> Entities { get; set; }

        public HashSet<T> Updates { get; set; }

        public void MakePersistent(T entity)
        {
            Entities.Add(entity);
        }

        public void MakeTransient(T entity)
        {
            Entities.Remove(entity);
        }

        public void Update(T entity)
        {
            Updates.Add(entity);
        }
    }
}