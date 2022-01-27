using System;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class WorkElementResponse
    {
        public int Id { get; set; }

        public PayElementTypeResponse PayElementType { get; set; }

        public string WorkOrder { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public string OperativeId { get; set; }

        public string OperativeName { get; set; }

        public WeekResponse Week { get; set; }

        public decimal Value { get; set; }

        public DateTime? ClosedAt { get; set; }
    }
}
