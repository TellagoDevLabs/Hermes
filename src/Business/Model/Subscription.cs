using System;

namespace TellagoStudios.Hermes.Business.Model
{
    public class Subscription : EntityBase
    {
        public Identity? TargetId { get; set; }
        public TargetKind TargetKind { get; set; }
        public Callback Callback { get; set; }
    }
}
