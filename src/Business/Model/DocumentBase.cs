namespace TellagoStudios.Hermes.Business.Model
{
    public class DocumentBase
    {
        public Identity? Id { get; set; }

        private bool Equals(DocumentBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id.HasValue && other.Id.Equals(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((DocumentBase) obj);
        }

        public override int GetHashCode()
        {
            return (Id.HasValue ? Id.Value.GetHashCode() : 0);
        }
    }
}