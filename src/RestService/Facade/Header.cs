namespace TellagoStudios.Hermes.RestService.Facade
{
    public class Header
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Name + ": " + Value;
        }
    }
}
