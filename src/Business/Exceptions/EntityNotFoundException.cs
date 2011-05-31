using System;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Business.Exceptions
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(Type type, Identity id)
            : base(string.Format(Messages.EntityNotFound, type.Name, id))
        {
        }
    }
}

