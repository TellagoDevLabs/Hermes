using System;

namespace TellagoStudios.Hermes.Client.Tests.IntegrationTests
{
    [Serializable]
    public class News
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as News;
            
            return other!=null &&
                other.Title == Title &&
                other.Date == Date;
        }

        public override int GetHashCode()
        {
            return Title==null ? 0 : Title.GetHashCode();
        }
    }
}
