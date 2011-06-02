namespace TellagoStudios.Hermes.Business.Model
{
    public class Group : DocumentBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Identity? ParentId { get; set; }
    }
}