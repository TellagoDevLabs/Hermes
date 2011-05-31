using System;

namespace TellagoStudios.Hermes.Common.Model
{
    public class Subscription
    {
        public Guid? Id { get; set; }
        public Guid? TargetId { get; set; }
        public TargetKind TargetKind { get; set; }
        public string Filter { get; set; }
        public Callback Callback { get; set; }
    }
}
