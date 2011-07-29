namespace TellagoStudios.Hermes.Business.Model
{
    public class Topic : EntityBase
    {
        public Identity? GroupId { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
    }
}