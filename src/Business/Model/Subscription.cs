using System;

namespace TellagoStudios.Hermes.Business.Model
{
    public class Subscription
    {
        public Identity? Id { get; set; }
        public Identity? TargetId { get; set; }
        public TargetKind TargetKind { get; set; }
        public string Filter { get; set; }
        public Callback Callback { get; set; }
    }
}
