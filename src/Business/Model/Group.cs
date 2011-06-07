namespace TellagoStudios.Hermes.Business.Model
{
    public class Group : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Identity? ParentId { get; set; }
    }
}