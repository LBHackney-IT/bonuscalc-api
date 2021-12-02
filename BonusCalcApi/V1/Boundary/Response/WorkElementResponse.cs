using System;

namespace BonusCalcApi.V1.Boundary.Response
{
    public class WorkElementResponse
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string WorkOrder { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public string OperativeId { get; set; }

        public string OperativeName { get; set; }

        public string WeekId { get; set; }

        public decimal Value { get; set; }

        public DateTime? ClosedAt { get; set; }
    }
}
