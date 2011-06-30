namespace TellagoStudios.Hermes.Client.Util
{
    public class Header
    {
        public Header()
        {
        }

        public Header(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Name + ": " + Value;
        }
    }
}
