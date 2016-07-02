using System;

namespace CallCenter.Model
{    
    public sealed class Call
    {
        public Guid Id { get; set; }
        public DateTime CallDate { get; set; }
        public double? OrderCost { get; set; }
        public string CallReport { get; set; }

        public Guid PersonId { get; set; }
    }
}