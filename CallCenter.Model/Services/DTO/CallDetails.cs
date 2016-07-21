using System;

namespace CallCenter.Model.Services.DTO
{
    public class CallDetails
    {
        public Guid Id { get; set; }
        public DateTime CallDate { get; set; }
        public double? OrderCost { get; set; }
        public string CallReport { get; set; }
    }
}
