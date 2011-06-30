namespace TellagoStudios.Hermes.Client
{
    public class ModelBase
    {
        public string Id { get; internal set; }

        public bool IsPersisted
        {
            get { return !string.IsNullOrEmpty(Id); }
        }

        public bool Equals(ModelBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Id, Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ModelBase)) return false;
            return Equals((ModelBase) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }

    public class Group : ModelBase
    {
        public Group(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public Group(string name)
            : this(name, string.Empty)
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }


    public class Topic : ModelBase
    {
        public Topic(
            string name, 
            string description, 
            Group @group)
        {
            Name = name;
            Description = description;
            Group = @group;
        }

        public Topic(string name, Group @group)
            : this(name, string.Empty, @group)
        {}

        public string Name { get; set; }
        public string Description { get; set; }
        public Group Group { get; set; }
    }
}