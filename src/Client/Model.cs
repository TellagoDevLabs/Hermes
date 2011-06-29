namespace TellagoStudios.Hermes.Client
{
    public class Group
    {
        public Group(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Id { get; internal set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public bool IsPersisted
        {
            get { return !string.IsNullOrEmpty(Id); }
        }
    }
}