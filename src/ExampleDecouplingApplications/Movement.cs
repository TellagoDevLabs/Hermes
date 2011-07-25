using System;

namespace ExampleDecouplingApplications
{
    [Serializable]
    public class Movement
    {
        public int AccountId { get; set; }
        public string Description { get; set;}
        public decimal Amount { get; set; }
    }
}
