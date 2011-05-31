using System;

namespace TellagoStudios.Hermes.Business.Model
{
    public class Group
    {
        public Identity? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Identity? ParentId { get; set; }
    }
}