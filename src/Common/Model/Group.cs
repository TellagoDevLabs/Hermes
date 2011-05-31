using System;

namespace TellagoStudios.Hermes.Common.Model
{
    public class Group
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
    }
}